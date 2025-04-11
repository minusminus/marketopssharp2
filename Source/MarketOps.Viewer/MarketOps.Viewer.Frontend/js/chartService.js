// js/chartService.js
import { getOhlcvLayout, getPriceTrace, getVolumeTrace } from './chartStyleConfig.js'; // Import konfiguracji

// Importuj moduły wskaźników
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
    currentChartData: null,
    currentStockInfo: null,
    currentChartType: 'candlestick', // Zapamiętajmy aktualny typ
    chartDivId: null,
    indicatorState: {
        SMA: { enabled: false, period: 20, data: null, traceIndexes: [] },
        Channel: { enabled: false, period: 20, data: null, traceIndexes: [] },
        BB: { enabled: false, period: 20, stdDev: 2, data: null, traceIndexes: [] },
        ATR: { enabled: false, period: 14, data: null, traceIndexes: [], yaxis: 'y3' } // ATR będzie na osi y3
    },
    calculatedIndicatorData: {}, // Zmieniamy na obiekt, klucze to np. 'SMA', 'BB'

    drawChart(chartDivId, apiData, stockInfo) {
		this.clearChart(chartDivId); // To wywołuje _resetIndicatorState
		
        this.currentChartData = apiData;
        this.currentStockInfo = stockInfo;
        this.chartDivId = chartDivId;
        this.currentChartType = 'candlestick'; // Resetuj do domyślnego przy nowym rysowaniu
		this._resetIndicatorState();

        if (!apiData || apiData.length === 0) {
            console.error("Brak danych do narysowania wykresu.");
            this.clearChart(chartDivId);
            return false;
        }
        console.log("Rysowanie wykresu Plotly...");
        try {
			this.calculateIndicators();
			
            const dates = apiData.map(item => item.timestamp.substring(0, 10));
            const opens = apiData.map(item => item.open);
            const highs = apiData.map(item => item.high);
            const lows = apiData.map(item => item.low);
            const closes = apiData.map(item => item.close);
            const volumes = apiData.map(item => item.volume); // Pamiętaj o typie int?

            // Użyj zaimportowanych funkcji do stworzenia śladów i layoutu
            const priceTrace = getPriceTrace(this.currentChartType, dates, opens, highs, lows, closes, stockInfo);
            const volumeTrace = getVolumeTrace(dates, volumes);
			const traces = [priceTrace, volumeTrace];
            const layout = this._getLayoutWithIndicators(); // Nowa funkcja tworząca layout

            const chartElement = document.getElementById(chartDivId);
            if (!chartElement) {
                console.error(`Element o ID '${chartDivId}' nie został znaleziony do rysowania.`);
                return false;
            }

            Plotly.newPlot(chartElement, [priceTrace, volumeTrace], layout, { responsive: true });
            console.log("Wykres narysowany.");
			
            // Opcjonalnie: Jeśli chcesz, aby wskaźniki były aktywne przy starcie, jeśli ich checkboxy są zaznaczone:
            // this._checkInitialIndicatorStates(); // Wywołaj funkcję sprawdzającą checkboxy
			
            return true;
        } catch (error) {
            console.error("Błąd podczas przygotowywania lub rysowania wykresu Plotly:", error);
            this.clearChart(chartDivId);
            return false;
        }
    },

    updateChartType(newType) {
        if (!this.chartDivId || !this.currentChartData || this.currentChartData.length === 0 || newType === this.currentChartType) {
            console.warn("Nie można zaktualizować typu wykresu - brak danych, wykresu lub typ jest taki sam.");
            return;
        }

        console.log(`Aktualizacja typu wykresu do: ${newType}`);
        const chartElement = document.getElementById(this.chartDivId);
        if (!chartElement) {
            console.error(`Element o ID '${this.chartDivId}' nie został znaleziony do aktualizacji.`);
            return;
        }

        // Pobierz istniejące dane (nie musimy ich ponownie mapować)
        const apiData = this.currentChartData;
        const dates = apiData.map(item => item.timestamp.substring(0, 10));
        const opens = apiData.map(item => item.open);
        const highs = apiData.map(item => item.high);
        const lows = apiData.map(item => item.low);
        const closes = apiData.map(item => item.close);

        // Użyj zaimportowanej funkcji do stworzenia NOWEJ definicji śladu ceny
        let priceTraceDefinition;
        try {
             priceTraceDefinition = getPriceTrace(newType, dates, opens, highs, lows, closes, this.currentStockInfo);
        } catch (error) {
             console.error("Błąd podczas tworzenia definicji śladu ceny:", error);
             return;
        }


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
            // Zaktualizuj tylko pierwszy ślad (o indeksie 0)
            Plotly.restyle(chartElement, updateData, [0]);
			
			// // Wymuś ponowne zastosowanie ustawień osi X z layoutu
            // // Pobieramy aktualny layout (mógł się zmienić np. przez zoom) i aktualizujemy tylko xaxis
            // const currentLayout = chartElement.layout || {}; // Pobierz istniejący layout lub pusty obiekt
            // const newXaxisLayout = getOhlcvLayout(this.currentStockInfo).xaxis; // Pobierz definicję xaxis z konfiguracji

            // // Zaktualizuj tylko oś X w istniejącym layoucie
            // const layoutUpdate = {
                 // 'xaxis.type': newXaxisLayout.type,
                 // 'xaxis.rangebreaks': newXaxisLayout.rangebreaks,
                 // 'xaxis.tickangle': newXaxisLayout.tickangle // Dodaj inne potrzebne atrybuty osi X
                 // // 'xaxis.nticks': newXaxisLayout.nticks // Jeśli używasz
            // };
            // Plotly.relayout(chartElement, layoutUpdate);
			
			// Zapisz nowy typ
            this.currentChartType = newType; 
            console.log("Typ wykresu zaktualizowany.");
        } catch (error) {
            console.error("Błąd podczas aktualizacji typu wykresu (Plotly.restyle):", error);
            // W razie błędu można spróbować przerysować cały wykres
            // this.drawChart(this.chartDivId, this.currentChartData, this.currentStockInfo);
        }
    },
	
    _resetIndicatorState() {
        for (const key in this.indicatorState) {
            this.indicatorState[key].enabled = false;
            this.indicatorState[key].data = null;
            this.indicatorState[key].traceIndexes = []; // Wyczyść indeksy śladów
        }
        this.calculatedIndicatorData = null;
    },
	
    calculateIndicators() {
        if (!this.currentChartData || this.currentChartData.length === 0) {
            this.calculatedIndicatorData = null;
            return;
        }
        // Sprawdź ponownie, czy funkcje są dostępne
         if (typeof sma !== 'function' || typeof bollingerbands !== 'function' || typeof atr !== 'function') {
             console.error("Funkcje wskaźników niedostępne - nie można obliczyć.");
             this.calculatedIndicatorData = null;
             return;
         }		
		
        console.log("Obliczanie wskaźników...");
        const data = this.currentChartData;
        // Przygotuj dane wejściowe dla biblioteki technicalindicators
        const input = {
            open: data.map(d => d.open),
            high: data.map(d => d.high),
            low: data.map(d => d.low),
            close: data.map(d => d.close),
            volume: data.map(d => d.volume),
            timestamp: data.map(d => new Date(d.timestamp)) // Biblioteka może potrzebować obiektów Date
        };

        this.calculatedIndicatorData = {}; // Zresetuj obliczone dane

        for (const key in indicatorModules) {
            const module = indicatorModules[key];
            const state = this.indicatorState[key];
            if (module && typeof module.calculate === 'function') {
                // Przekaż odpowiednie parametry do funkcji calculate
                let result;
                if (key === 'BB') {
                    result = module.calculate(input, state.period, state.stdDev);
                } else if (key === 'ATR' || key === 'Channel' || key === 'SMA'){ // Dostosuj, jeśli inne wskaźniki potrzebują specyficznych parametrów
                     result = module.calculate(input, state.period);
                } else {
                     result = module.calculate(input, state.period); // Domyślnie przekazuj okres
                }
                this.calculatedIndicatorData[key] = result; // Zapisz wynik (może być null)
                if (!result) {
                    console.warn(`Nie udało się obliczyć wskaźnika: ${key}`);
                }
            }
        }
        console.log("Wskaźniki obliczone (lub próba obliczenia zakończona).");
    },

     toggleIndicator(indicatorKey, isEnabled) {
         if (!this.chartDivId) {
             console.warn("Nie można przełączyć wskaźnika - brak wykresu.");
             return;
         }

        const state = this.indicatorState[indicatorKey];
        const module = indicatorModules[indicatorKey]; // Pobierz odpowiedni moduł

        if (!state || !module) {
            console.error(`Nieznany klucz wskaźnika lub brak modułu: ${indicatorKey}`);
            return;
        }
        if (state.enabled === isEnabled) return;
		 
        // Oblicz wskaźniki, jeśli jeszcze nie ma dla TEGO klucza
        // (lub jeśli logika wymaga ponownego obliczenia przy zmianie parametrów - na razie nie mamy)
        if (!this.calculatedIndicatorData || this.calculatedIndicatorData[indicatorKey] === undefined) {
             console.warn(`Dane dla wskaźnika ${indicatorKey} nieobliczone. Próbuję obliczyć wszystkie.`);
             this.calculateIndicators();
        }
        // Sprawdź ponownie
        const indicatorResult = this.calculatedIndicatorData[indicatorKey]; // Może być null, jeśli obliczenie zawiodło

        state.enabled = isEnabled;
        const chartElement = document.getElementById(this.chartDivId);
        if (!chartElement) return;
        console.log(`Przełączanie wskaźnika ${indicatorKey} na ${isEnabled}`);

        if (isEnabled) {
            // --- Dodawanie wskaźnika ---
             if (indicatorResult === null) { // Sprawdź, czy obliczenie się powiodło
                 console.warn(`Nie można dodać wskaźnika ${indicatorKey}, brak poprawnych danych.`);
                 state.enabled = false; // Cofnij zmianę stanu
                 // TODO: Odznacz checkbox w UI?
                 return;
             }

            let tracesToAdd = [];
            const allDates = this.currentChartData.map(item => item.timestamp.substring(0, 10));

            try {
                // Wywołaj getTraces z odpowiedniego modułu
                tracesToAdd = module.getTraces(allDates, indicatorResult, state);

                 if (tracesToAdd && tracesToAdd.length > 0) {
                    Plotly.addTraces(chartElement, tracesToAdd).then(() => {
                        // ... (aktualizacja traceIndexes - logika bez zmian) ...
                        const currentTraceCount = chartElement.data ? chartElement.data.length : 0;
                        state.traceIndexes = [];
                        for (let i = 0; i < tracesToAdd.length; i++) {
                            state.traceIndexes.push(currentTraceCount - tracesToAdd.length + i);
                        }
                        console.log(`Dodano ślady dla ${indicatorKey}, nowe indeksy:`, state.traceIndexes);
                    });
                     // Aktualizuj layout TYLKO jeśli to ATR (lub inny wskaźnik wymagający zmiany layoutu)
                     if (indicatorKey === 'ATR') {
                        const layoutUpdate = this._getLayoutWithIndicators();
                        Plotly.relayout(chartElement, layoutUpdate);
                     }
                 } else {
                      console.warn(`Moduł ${indicatorKey} nie zwrócił śladów do dodania.`);
                      state.enabled = false; // Cofnij zmianę stanu
                 }
            } catch (error) {
                 console.error(`Błąd podczas generowania śladów dla ${indicatorKey}:`, error);
                 state.enabled = false;
            }

        } else {
            // --- Usuwanie wskaźnika ---
            if (state.traceIndexes && state.traceIndexes.length > 0) {
                const sortedIndexes = [...state.traceIndexes].sort((a, b) => b - a);
                Plotly.deleteTraces(chartElement, sortedIndexes).then(() => {
                    console.log(`Usunięto ślady dla ${indicatorKey}, indeksy:`, sortedIndexes);
                    state.traceIndexes = [];
                    if (indicatorKey === 'ATR') {
                         const layoutUpdate = this._getLayoutWithIndicators();
                         Plotly.relayout(chartElement, layoutUpdate);
                    }
                });
            } else { /* ... */ }
        }
     },

     _getLayoutWithIndicators() {
         // Zacznij od bazowego layoutu
         const baseLayout = getOhlcvLayout(this.currentStockInfo);

         // Sprawdź, czy ATR jest włączony
         if (this.indicatorState.ATR.enabled) {
              // Dostosuj domeny osi Y, aby zrobić miejsce na ATR
              baseLayout.yaxis.domain = [0.35, 1]; // Cena: 35% - 100%
              baseLayout.yaxis2.domain = [0.15, 0.30]; // Wolumen: 15% - 30%
              // Dodaj definicję osi Y3 dla ATR
              baseLayout.yaxis3 = {
                    ...baseLayout.yaxis.titlefont ? { titlefont: baseLayout.yaxis.titlefont } : {}, // Kopiuj style fontów jeśli istnieją
                    ...baseLayout.yaxis.tickfont ? { tickfont: baseLayout.yaxis.tickfont } : {},
                    title: `ATR(${this.indicatorState.ATR.period})`,
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

    clearChart(chartDivId) {
		if (!chartDivId) return;
		this._resetIndicatorState(); // Resetuj stan wskaźników przy czyszczeniu
		const chartElement = document.getElementById(chartDivId);
		if (chartElement) {
			try {
				Plotly.purge(chartElement);
				console.log(`Wykres w divie #${chartDivId} wyczyszczony.`);
			} catch (error) {
				console.warn(`Problem podczas czyszczenia wykresu #${chartDivId}: ${error.message}`);
				chartElement.innerHTML = '';
			}
		}
		this.currentChartData = null;
		this.currentStockInfo = null;
		this.currentChartType = 'candlestick'; // Resetuj typ
		this.chartDivId = null;
    }
	
    // Opcjonalna funkcja do sprawdzania checkboxów przy starcie
    /*
    _checkInitialIndicatorStates() {
        if (!this.chartDivId) return;
        console.log("Sprawdzanie początkowego stanu wskaźników...");
        // Pobierz stan checkboxów z uiHandler (potrzebna by była metoda w uiHandler zwracająca stan)
        // const initialStates = uiHandler.getIndicatorCheckboxStates();
        // for (const key in initialStates) {
        //     if (initialStates[key]) {
        //          console.log(`Wskaźnik ${key} zaznaczony przy starcie - próba aktywacji.`);
        //          this.toggleIndicator(key, true);
        //     }
        // }
    }
    */	
};

export { chartService };
