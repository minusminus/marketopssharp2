// js/chartService.js
import { getOhlcvLayout, getPriceTrace, getVolumeTrace } from './chartStyleConfig.js'; // Import konfiguracji
import { padDataForPlotly, padBBDataForPlotly } from './indicators/indicatorUtils.js';
import { smaIndicator } from './indicators/smaIndicator.js';
import { channelIndicator } from './indicators/channelIndicator.js';
import { bbIndicator } from './indicators/bbIndicator.js';
import { atrIndicator } from './indicators/atrIndicator.js';

// Mapowanie kluczy stanu na moduły wskaźników
const indicatorModules = {
    SMA: smaIndicator,
    Channel: channelIndicator,
    BB: bbIndicator,
    ATR: atrIndicator
};

const chartService = {
    // Używamy obiektu, gdzie kluczem jest ID zakładki (lub ID diva plotly)
    chartsState: {}, // przechowuje { chartDivId: { data, stockInfo, indicators, layout, ... } }

    _getChartState(tabId) {
        if (!this.chartsState[tabId]) {
            // Inicjalizuj stan dla nowej zakładki
            this.chartsState[tabId] = {
                chartDivId: `plotly-chart-${tabId}`, // Od razu ustawmy ID diva
                chartElement: null,
                currentChartData: null,
                currentStockInfo: null,
                currentChartType: 'candlestick',
                indicatorState: this._getInitialIndicatorState(), // Kopia początkowego stanu wskaźników
                calculatedIndicatorData: {},
                currentLayout: null
            };
        }
        return this.chartsState[tabId];
    },

    _getInitialIndicatorState() {
        // Zwraca głęboką kopię, aby każda zakładka miała niezależny stan
        return JSON.parse(JSON.stringify({
            SMA: { enabled: false, period: 20, traceIndexes: [] },
            Channel: { enabled: false, period: 20, traceIndexes: [] },
            BB: { enabled: false, period: 20, stdDev: 2, traceIndexes: [] },
            ATR: { enabled: false, period: 14, yaxis: 'y3', traceIndexes: [] } // Zawsze przypisz yaxis tutaj
        }));
    },

    drawChart(tabId, apiData, stockInfo) {
        const state = this._getChartState(tabId); // Pobierz lub utwórz stan dla tej zakładki

        state.currentChartData = apiData;
        state.currentStockInfo = stockInfo;
        state.currentChartType = 'candlestick';
        state.indicatorState = this._getInitialIndicatorState(); // Resetuj wskaźniki
        state.calculatedIndicatorData = {}; // Resetuj obliczone

        state.chartElement = document.getElementById(state.chartDivId);
        if (!state.chartElement) {
             console.error(`Element plotly chart div not found for tab ${tabId} with ID ${state.chartDivId}`);
             return false;
        }
        // Wyczyść poprzednią zawartość Plotly, jeśli istniała
        try {
             if (state.chartElement.layout) { // Sprawdź, czy był tam wykres Plotly
                 console.log(`[${tabId}] drawChart: Czyszczenie poprzedniego wykresu (Plotly.purge).`);
                 Plotly.purge(state.chartElement);
             }
         } catch(e) { console.warn(`[${tabId}] drawChart: Błąd podczas purge: ${e.message}`); }
		
        if (!apiData || apiData.length === 0) {
            console.error("Brak danych do narysowania wykresu.");
            return false;
        }
		
        try {
			this.calculateIndicators(tabId);
			
            const dates = apiData.map(item => item.timestamp.substring(0, 10));
            const opens = apiData.map(item => item.open);
            const highs = apiData.map(item => item.high);
            const lows = apiData.map(item => item.low);
            const closes = apiData.map(item => item.close);
            const volumes = apiData.map(item => item.volume); // Pamiętaj o typie int?

            // Użyj zaimportowanych funkcji do stworzenia śladów i layoutu
            const priceTrace = getPriceTrace(state.currentChartType, dates, opens, highs, lows, closes, stockInfo);
            const volumeTrace = getVolumeTrace(dates, volumes);
			const traces = [priceTrace, volumeTrace];
            const layout = this._getLayoutWithIndicators(tabId, state.indicatorState); // Layout zależy od stanu wskaźników zakładki
            state.currentLayout = layout; // Zapisz layout w stanie

            Plotly.newPlot(state.chartElement, traces, layout, { responsive: true });
			console.log(`[${tabId}] drawChart: Wykres narysowany. Stan:`, JSON.stringify(Object.keys(this.chartsState))); // Pokaż klucze w stanie globalnym
            return true;
        } catch (error) {
            console.error("Błąd podczas przygotowywania lub rysowania wykresu Plotly:", error);
            this.clearChart(state.chartDivId);
            return false;
        }
    },

    updateChartType(tabId, newType) {
        const state = this._getChartState(tabId);
         if (!state.chartElement || !state.currentChartData || newType === state.currentChartType) {
              console.warn(`updateChartType: Warunki niespełnione lub brak elementu dla ${tabId}`);
              return;
         }

        const apiData = state.currentChartData;
        const dates = apiData.map(item => item.timestamp.substring(0, 10));
        const opens = apiData.map(item => item.open);
        const highs = apiData.map(item => item.high);
        const lows = apiData.map(item => item.low);
        const closes = apiData.map(item => item.close);

        // Użyj zaimportowanej funkcji do stworzenia NOWEJ definicji śladu ceny
        let priceTraceDefinition = getPriceTrace(newType, dates, opens, highs, lows, closes, state.currentStockInfo);

        // Przygotuj obiekt aktualizacji dla Plotly.restyle
        // Potrzebujemy tylko tych pól, które się ZMIENIAJĄ między typami
        const updateData = {
            type: [priceTraceDefinition.type],
            // Dla scatter musimy ustawić 'y' i 'mode', dla candlestick te dane są w innych polach
            y: [priceTraceDefinition.y], // Będzie miało wartość tylko dla scatter
            mode: [priceTraceDefinition.mode], // Będzie miało wartość tylko dla scatter
            // Dane OHLC tylko dla candlestick
            open: [priceTraceDefinition.open],
            high: [priceTraceDefinition.high],
            low: [priceTraceDefinition.low],
            close: [priceTraceDefinition.close],
            // Style specyficzne dla typów
            line: [priceTraceDefinition.line], // Tylko dla scatter
            increasing: [priceTraceDefinition.increasing], // Tylko dla candlestick
            decreasing: [priceTraceDefinition.decreasing] // Tylko dla candlestick
        };

        try {
            Plotly.restyle(state.chartElement, updateData, [0]);
            state.currentChartType = newType; 
        } catch (error) {
            console.error("Błąd podczas aktualizacji typu wykresu (Plotly.restyle):", error);
            // W razie błędu można spróbować przerysować cały wykres
            // this.drawChart(this.chartDivId, this.currentChartData, this.currentStockInfo);
        }
    },
	
    calculateIndicators(tabId) {
        const state = this._getChartState(tabId);
        if (!state.currentChartData || state.currentChartData.length === 0) {
            state.calculatedIndicatorData = {}; return;
        }
        const data = state.currentChartData;
        // Przygotuj dane wejściowe dla biblioteki technicalindicators
        const input = {
            open: data.map(d => d.open),
            high: data.map(d => d.high),
            low: data.map(d => d.low),
            close: data.map(d => d.close),
            volume: data.map(d => d.volume),
            timestamp: data.map(d => new Date(d.timestamp)) // Biblioteka może potrzebować obiektów Date
        };

        state.calculatedIndicatorData = {}; // Zresetuj obliczone dane

        for (const key in indicatorModules) {
            const module = indicatorModules[key];
            const indicatorConfig = state.indicatorState[key];
            if (module && typeof module.calculate === 'function') {
                // Przekaż odpowiednie parametry do funkcji calculate
                let result;
                if (key === 'BB') {
                    result = module.calculate(input, indicatorConfig.period, indicatorConfig.stdDev);
                } else {
                     result = module.calculate(input, indicatorConfig.period); // Domyślnie przekazuj okres
                }
				
                state.calculatedIndicatorData[key] = result; // Zapisz wynik (może być null)
                if (!result) {
                    console.warn(`Nie udało się obliczyć wskaźnika: ${key}`);
                }
            }
        }
    },

    toggleIndicator(tabId, indicatorKey, isEnabled) {
        const state = this._getChartState(tabId);
		
         if (!state.chartElement) { console.warn(`toggleIndicator: Brak elementu wykresu dla ${tabId}`); return; }
         if (!state.currentChartData || state.currentChartData.length === 0) { console.warn(`toggleIndicator: Brak danych wykresu dla ${tabId}`); return; }

         // Sprawdź, czy dane dla wykresu w ogóle istnieją
         if (!state.currentChartData || state.currentChartData.length === 0) {
             console.warn(`toggleIndicator: Brak danych wykresu dla ${tabId}`);
             return; // Nie rób nic, jeśli nie ma danych bazowych
         }
        const module = indicatorModules[indicatorKey];
        if (!state || !module) {
            console.error(`Nieznany klucz wskaźnika lub brak modułu: ${indicatorKey}`);
            return;
        }
        const indicatorConfig = state.indicatorState[indicatorKey];
        if (!indicatorConfig || indicatorConfig.enabled === isEnabled) return;
		 
         // Obliczenia powinny być już zrobione w drawChart, ale dodajmy sprawdzenie
         if (state.calculatedIndicatorData[indicatorKey] === undefined) {
             console.warn(`Dane dla ${indicatorKey} w ${tabId} nieobliczone. Próbuję obliczyć ponownie.`);
             this.calculateIndicators(tabId); // Oblicz ponownie dla tej zakładki
         }
        const indicatorResult = state.calculatedIndicatorData[indicatorKey];
        indicatorConfig.enabled = isEnabled; // Zmień stan dla TEJ zakładki

        if (isEnabled) {
            // --- Dodawanie wskaźnika ---
             if (indicatorResult === null || indicatorResult === undefined) { // Dokładniejsze sprawdzenie
                 console.warn(`Nie można dodać wskaźnika ${indicatorKey} w ${tabId}, brak poprawnych danych.`);
                 indicatorConfig.enabled = false; // Cofnij
                 return;
             }

            let tracesToAdd = [];
			const allDates = state.currentChartData.map(item => item.timestamp.substring(0, 10));

            try {
                // Wywołaj getTraces z odpowiedniego modułu
                tracesToAdd = module.getTraces(allDates, indicatorResult, indicatorConfig);

                 if (tracesToAdd && tracesToAdd.length > 0) {
                    Plotly.addTraces(state.chartElement, tracesToAdd).then(() => {
                        const currentTraceCount = state.chartElement.data ? state.chartElement.data.length : 0;
                        indicatorConfig.traceIndexes = [];
                        for (let i = 0; i < tracesToAdd.length; i++) {
                            indicatorConfig.traceIndexes.push(currentTraceCount - tracesToAdd.length + i);
                        }
                    });
                     // Aktualizuj layout TYLKO jeśli to ATR (lub inny wskaźnik wymagający zmiany layoutu)
                     if (indicatorKey === 'ATR') {
                        const layoutUpdate = this._getLayoutWithIndicators(tabId, state.indicatorState);
                        Plotly.relayout(state.chartElement, layoutUpdate);
                     }
                 } else {
                      console.warn(`Moduł ${indicatorKey} nie zwrócił śladów do dodania.`);
                      indicatorConfig.enabled = false; // Cofnij zmianę stanu
                 }
            } catch (error) {
                 console.error(`Błąd podczas generowania śladów dla ${indicatorKey}:`, error);
                 indicatorConfig.enabled = false;
            }

        } else {
            // --- Usuwanie wskaźnika ---
            if (indicatorConfig.traceIndexes && indicatorConfig.traceIndexes.length > 0) {
                const sortedIndexes = [...indicatorConfig.traceIndexes].sort((a, b) => b - a);
                Plotly.deleteTraces(state.chartElement, sortedIndexes).then(() => {
                    indicatorConfig.traceIndexes = [];
                    if (indicatorKey === 'ATR') {
                         const layoutUpdate = this._getLayoutWithIndicators(tabId, state.indicatorState);
                         Plotly.relayout(state.chartElement, layoutUpdate);
                    }
                });
            } else { /* ... */ }
        }
     },

     _getLayoutWithIndicators(tabId, currentIndicatorState) {
        const state = this._getChartState(tabId);
        const baseLayout = getOhlcvLayout(state.currentStockInfo); // Użyj info ze stanu zakładki

         // Sprawdź, czy ATR jest włączony
         if (currentIndicatorState.ATR.enabled) {
              // Dostosuj domeny osi Y, aby zrobić miejsce na ATR
              baseLayout.yaxis.domain = [0.35, 1]; // Cena: 35% - 100%
              baseLayout.yaxis2.domain = [0.15, 0.30]; // Wolumen: 15% - 30%
              // Dodaj definicję osi Y3 dla ATR
              baseLayout.yaxis3 = {
                    ...baseLayout.yaxis.titlefont ? { titlefont: baseLayout.yaxis.titlefont } : {}, // Kopiuj style fontów jeśli istnieją
                    ...baseLayout.yaxis.tickfont ? { tickfont: baseLayout.yaxis.tickfont } : {},
                    title: `ATR(${currentIndicatorState.ATR.period})`,
                    domain: [0, 0.10], // Dolne 10%
                    autorange: true,
                    showgrid: true,
                    gridcolor: '#e1e1e1'
              };
         } else {
              // Przywróć domyślne domeny, jeśli ATR jest wyłączony
               baseLayout.yaxis.domain = [0.25, 1];
               baseLayout.yaxis2.domain = [0, 0.2];
               // Usuń oś yaxis3, jeśli istnieje
               delete baseLayout.yaxis3;
         }

         return baseLayout;
     },

    clearChart(tabId) {
        const state = this.chartsState[tabId];
        if (state && state.chartElement) {
            // this._removeMouseListeners(state.chartElement); // Jeśli są
            try { Plotly.purge(state.chartElement); } catch(e) {}
        } else {
             // console.log(`[${tabId}] clearChart: Brak stanu lub elementu do wyczyszczenia.`);
        }
        delete this.chartsState[tabId]; // Usuń stan
    },

    // Modyfikacja _removeMouseListeners, aby przyjmowała element
     _removeMouseListeners(chartElement) {
         if (!chartElement) return;
         // Usuń specyficzne listenery (jeśli przechowujesz referencje jak _boundHandleMouseMove)
         // Lub ogólnie:
         // chartElement.removeEventListener('mousemove', ...);
         // chartElement.removeEventListener('mouseleave', ...);
         // chartElement.off('plotly_relayout', ...); // Jeśli używasz nowszej wersji Plotly z .on/.off
          console.warn("_removeMouseListeners needs implementation if mouse listeners were added.");
     },
};

export { chartService };
