// js/chartService.js
import { getOhlcvLayout, getPriceTrace, getVolumeTrace } from './chartStyleConfig.js'; // Import konfiguracji
import { getSmaTrace, getPriceChannelTraces, getBollingerBandsTraces, getAtrTrace } from './chartStyleConfig.js';

// Upewnijmy się, że biblioteka jest dostępna globalnie
// Sprawdź istnienie kilku kluczowych funkcji bezpośrednio w globalnym zakresie (window)
if (typeof sma !== 'function' || typeof bollingerbands !== 'function' || typeof atr !== 'function') {
    console.error("Wygląda na to, że funkcje biblioteki technicalindicators nie są dostępne globalnie!");
    // Zablokuj funkcjonalność wskaźników lub rzuć błąd
    // Można by stworzyć "pusty" obiekt ti, aby uniknąć błędów dalej, ale lepiej dać znać o problemie
}

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
    calculatedIndicatorData: null, // Obiekt przechowujący wszystkie obliczone wskaźniki

    drawChart(chartDivId, apiData, stockInfo) {
		this.clearChart(chartDivId); // To wywołuje _resetIndicatorState
		
        this.currentChartData = apiData;
        this.currentStockInfo = stockInfo;
        this.chartDivId = chartDivId;
        this.currentChartType = 'candlestick'; // Resetuj do domyślnego przy nowym rysowaniu

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

        try {
            // Oblicz SMA
            const smaPeriod = this.indicatorState.SMA.period;
            if (input.close.length >= smaPeriod) {
                this.calculatedIndicatorData.SMA = sma({ period: smaPeriod, values: input.close });
            } else { this.calculatedIndicatorData.SMA = []; }


            // Oblicz Kanał Cenowy (wymaga high i low)
            const channelPeriod = this.indicatorState.Channel.period;
             if (input.high.length >= channelPeriod && input.low.length >= channelPeriod) {
                 // Biblioteka technicalindicators nie ma wprost kanału Donchiana/Price Channel
                 // Musimy go zaimplementować ręcznie lub użyć innej logiki.
                 // Poniżej prosta implementacja: Max(High) i Min(Low) w okresie.
                 this.calculatedIndicatorData.Channel = { upper: [], lower: [] };
                 for (let i = 0; i < input.high.length; i++) {
                     if (i < channelPeriod - 1) {
                         this.calculatedIndicatorData.Channel.upper.push(null);
                         this.calculatedIndicatorData.Channel.lower.push(null);
                     } else {
                         const highsSlice = input.high.slice(i - channelPeriod + 1, i + 1);
                         const lowsSlice = input.low.slice(i - channelPeriod + 1, i + 1);
                         this.calculatedIndicatorData.Channel.upper.push(Math.max(...highsSlice));
                         this.calculatedIndicatorData.Channel.lower.push(Math.min(...lowsSlice));
                     }
                 }
             } else { this.calculatedIndicatorData.Channel = { upper: [], lower: [] }; }

            // Oblicz Bollinger Bands
            const bbPeriod = this.indicatorState.BB.period;
            const bbStdDev = this.indicatorState.BB.stdDev;
            if (input.close.length >= bbPeriod) {
                this.calculatedIndicatorData.BB = bollingerbands({ period: bbPeriod, values: input.close, stdDev: bbStdDev });
                 // Wynik BB to tablica obiektów { middle, upper, lower }
            } else { this.calculatedIndicatorData.BB = []; }

            // Oblicz ATR
            const atrPeriod = this.indicatorState.ATR.period;
             if (input.high.length >= atrPeriod && input.low.length >= atrPeriod && input.close.length >= atrPeriod) {
                const atrInput = { high: input.high, low: input.low, close: input.close, period: atrPeriod };
                this.calculatedIndicatorData.ATR = atr(atrInput);
            } else { this.calculatedIndicatorData.ATR = []; }


            console.log("Wskaźniki obliczone.");
            // console.log(this.calculatedIndicatorData); // Do debugowania

        } catch (error) {
            console.error("Błąd podczas obliczania wskaźników:", error);
            this.calculatedIndicatorData = null; // Wyczyść w razie błędu
        }
    },

     toggleIndicator(indicatorKey, isEnabled) {
         if (!this.chartDivId || !this.calculatedIndicatorData) {
             console.warn("Nie można przełączyć wskaźnika - brak wykresu lub obliczonych danych.");
             return;
         }
        // Oblicz wskaźniki, jeśli jeszcze nie zostały obliczone (np. przy pierwszym przełączeniu)
        if (!this.calculatedIndicatorData) {
             this.calculateIndicators();
        }
        // Sprawdź ponownie po obliczeniu
        if(!this.calculatedIndicatorData){
            console.warn("Nie udało się obliczyć danych wskaźników.");
            // Można spróbować odznaczyć checkbox w UI (trudniejsze)
            return;
        }
         if (!this.indicatorState[indicatorKey]) {
             console.error(`Nieznany klucz wskaźnika: ${indicatorKey}`);
             return;
         }

         const state = this.indicatorState[indicatorKey];
		 if (state.enabled === isEnabled) return;
         state.enabled = isEnabled; // Zaktualizuj stan
         const chartElement = document.getElementById(this.chartDivId);
         if (!chartElement) return;

         console.log(`Przełączanie wskaźnika ${indicatorKey} na ${isEnabled}`);

         if (isEnabled) {
             // --- Dodawanie wskaźnika ---
             let tracesToAdd = [];
             const allDates = this.currentChartData.map(item => item.timestamp.substring(0, 10));
			 const dataLength = allDates.length;

             try {
                 switch (indicatorKey) {
                    case 'SMA': { // Używamy bloku, aby 'period' i 'indicatorData' miały lokalny zasięg
                        const period = state.period;
                        // Pobierz wynik z biblioteki (może być krótszy)
                        const indicatorResult = this.calculatedIndicatorData.SMA;
                        if (indicatorResult) {
                            const validStartIndex = period - 1;
                            const nullPaddingCount = validStartIndex; // Liczba nulli do dodania
                            const expectedResultLength = dataLength - nullPaddingCount; // Oczekiwana długość wyniku z biblioteki

                            console.log(`SMA - Data L: ${dataLength}, Period: ${period}, Expected Result L: ${expectedResultLength}, Actual Result L: ${indicatorResult.length}`); // DEBUG

                            // Sprawdź, czy długość wyniku z biblioteki jest zgodna z oczekiwaniami
                            if (indicatorResult.length === expectedResultLength) {
                                // Stwórz tablicę z nullami na początku
                                const valuesForPlotly = Array(nullPaddingCount).fill(null).concat(indicatorResult);

                                console.log(`SMA Padded - Dates L: ${allDates.length}, Padded Values L: ${valuesForPlotly.length}`); // DEBUG

                                if(allDates.length === valuesForPlotly.length) {
                                    tracesToAdd.push(getSmaTrace(allDates, valuesForPlotly, period));
                                } else { console.error("SMA - Niezgodność długości po paddingu!"); }
                            } else {
                                console.warn(`SMA - Niezgodna długość wyniku z biblioteki (${indicatorResult.length} vs ${expectedResultLength} oczekiwano). Nie dodaję śladu.`);
                            }
                        } else { console.warn("SMA - Brak obliczonych danych."); }
                        break;
                    }
                    case 'Channel': {
                        const period = state.period;
                        const indicatorData = this.calculatedIndicatorData.Channel;
                        if (indicatorData && indicatorData.upper.length > 0) {
                            const validStartIndex = period - 1;
                            if (validStartIndex < allDates.length) {
                                const dates = allDates.slice(validStartIndex);
                                const upperValues = indicatorData.upper.slice(validStartIndex);
                                const lowerValues = indicatorData.lower.slice(validStartIndex);
                                tracesToAdd.push(...getPriceChannelTraces(dates, upperValues, lowerValues, period));
                            }
                        }
                        break;
                    }
                    case 'BB': {
                        const period = state.period;
                        const indicatorResult = this.calculatedIndicatorData.BB; // Tablica obiektów {middle, upper, lower}
                        if (indicatorResult) {
                            const validStartIndex = period - 1;
                            const nullPaddingCount = validStartIndex;
                            const expectedResultLength = dataLength - nullPaddingCount;

                             console.log(`BB - Data L: ${dataLength}, Period: ${period}, Expected Result L: ${expectedResultLength}, Actual Result L: ${indicatorResult.length}`); // DEBUG

                            if (indicatorResult.length === expectedResultLength) {
                                const valuesForPlotly = {
                                     middle: Array(nullPaddingCount).fill(null).concat(indicatorResult.map(d => d?.middle)),
                                     upper: Array(nullPaddingCount).fill(null).concat(indicatorResult.map(d => d?.upper)),
                                     lower: Array(nullPaddingCount).fill(null).concat(indicatorResult.map(d => d?.lower))
                                 };
                                console.log(`BB Padded - Dates L: ${allDates.length}, Padded Middle L: ${valuesForPlotly.middle.length}`); // DEBUG
                                if(allDates.length === valuesForPlotly.middle.length) {
                                    tracesToAdd.push(...getBollingerBandsTraces(allDates, valuesForPlotly, state.period, state.stdDev));
                                 } else { console.error("BB - Niezgodność długości po paddingu!"); }
                            } else {
                                 console.warn(`BB - Niezgodna długość wyniku z biblioteki (${indicatorResult.length} vs ${expectedResultLength} oczekiwano). Nie dodaję śladu.`);
                            }
                        } else { console.warn("BB - Brak obliczonych danych."); }
                        break;
                    }
                    case 'ATR': {
                        const period = state.period;
                        const indicatorResult = this.calculatedIndicatorData.ATR;
                        if (indicatorResult) {
                             // Zakładamy, że ATR zaczyna się od indeksu 'period'
                             const validStartIndex = period;
                             const nullPaddingCount = validStartIndex;
                             const expectedResultLength = dataLength - nullPaddingCount;

                             console.log(`ATR - Data L: ${dataLength}, Period: ${period}, Expected Result L: ${expectedResultLength}, Actual Result L: ${indicatorResult.length}`); // DEBUG

                             if (indicatorResult.length === expectedResultLength) {
                                const valuesForPlotly = Array(nullPaddingCount).fill(null).concat(indicatorResult);
                                console.log(`ATR Padded - Dates L: ${allDates.length}, Padded Values L: ${valuesForPlotly.length}`); // DEBUG

                                 if(allDates.length === valuesForPlotly.length) {
                                     tracesToAdd.push(getAtrTrace(allDates, valuesForPlotly, state.period, state.yaxis));
                                     const layoutUpdate = this._getLayoutWithIndicators();
                                     Plotly.relayout(chartElement, layoutUpdate);
                                 } else { console.error("ATR - Niezgodność długości po paddingu!"); }
                             } else {
                                 console.warn(`ATR - Niezgodna długość wyniku z biblioteki (${indicatorResult.length} vs ${expectedResultLength} oczekiwano). Nie dodaję śladu.`);
                             }
                         } else { console.warn("ATR - Brak obliczonych danych."); }
                        break;
                    }
                 }

                 if (tracesToAdd.length > 0) {
                     Plotly.addTraces(chartElement, tracesToAdd);
                     // Zapisz indeksy dodanych śladów (ważne dla usuwania)
                     const currentTraceCount = chartElement.data ? chartElement.data.length : 0;
                     state.traceIndexes = [];
                     for (let i = 0; i < tracesToAdd.length; i++) {
                         state.traceIndexes.push(currentTraceCount - tracesToAdd.length + i);
                     }
                     console.log(`Dodano ślady dla ${indicatorKey}, indeksy:`, state.traceIndexes);
                 } else {
                      console.warn(`Brak danych lub błąd przy tworzeniu śladu dla wskaźnika ${indicatorKey}`);
                       state.enabled = false; // Wyłącz, jeśli nie udało się dodać
                       // Można też odznaczyć checkbox w UI (wymagałoby komunikacji zwrotnej do uiHandler)
                 }
             } catch (error) {
                  console.error(`Błąd podczas dodawania wskaźnika ${indicatorKey}:`, error);
                  state.enabled = false; // Wyłącz w razie błędu
             }

         } else {
             // --- Usuwanie wskaźnika ---
             if (state.traceIndexes && state.traceIndexes.length > 0) {
                 Plotly.deleteTraces(chartElement, state.traceIndexes);
                 console.log(`Usunięto ślady dla ${indicatorKey}, indeksy:`, state.traceIndexes);
                 state.traceIndexes = []; // Wyczyść zapisane indeksy
             }
             // Jeśli usuwamy ATR, potencjalnie chcemy usunąć oś y3 i dostosować inne domeny
             if (indicatorKey === 'ATR') {
                  const layoutUpdate = this._getLayoutWithIndicators(); // Pobierz layout bez ATR
                  Plotly.update(chartElement, {}, layoutUpdate); // Zaktualizuj layout
             }
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
