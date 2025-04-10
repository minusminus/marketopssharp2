// js/chartService.js
import { getOhlcvLayout, getPriceTrace, getVolumeTrace } from './chartStyleConfig.js'; // Import konfiguracji

const chartService = {
    currentChartData: null,
    currentStockInfo: null,
    currentChartType: 'candlestick', // Zapamiętajmy aktualny typ
    chartDivId: null,

    drawChart(chartDivId, apiData, stockInfo) {
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
            const dates = apiData.map(item => item.timestamp.substring(0, 10));
            const opens = apiData.map(item => item.open);
            const highs = apiData.map(item => item.high);
            const lows = apiData.map(item => item.low);
            const closes = apiData.map(item => item.close);
            const volumes = apiData.map(item => item.volume); // Pamiętaj o typie int?

            // Użyj zaimportowanych funkcji do stworzenia śladów i layoutu
            const priceTrace = getPriceTrace(this.currentChartType, dates, opens, highs, lows, closes, stockInfo);
            const volumeTrace = getVolumeTrace(dates, volumes);
            const layout = getOhlcvLayout(stockInfo); // Layout jest teraz tworzony przez funkcję konfiguracyjną

            const chartElement = document.getElementById(chartDivId);
            if (!chartElement) {
                console.error(`Element o ID '${chartDivId}' nie został znaleziony do rysowania.`);
                return false;
            }

            Plotly.newPlot(chartElement, [priceTrace, volumeTrace], layout, { responsive: true });
            console.log("Wykres narysowany.");
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
			
			// Wymuś ponowne zastosowanie ustawień osi X z layoutu
            // Pobieramy aktualny layout (mógł się zmienić np. przez zoom) i aktualizujemy tylko xaxis
            const currentLayout = chartElement.layout || {}; // Pobierz istniejący layout lub pusty obiekt
            const newXaxisLayout = getOhlcvLayout(this.currentStockInfo).xaxis; // Pobierz definicję xaxis z konfiguracji

            // Zaktualizuj tylko oś X w istniejącym layoucie
            const layoutUpdate = {
                 'xaxis.type': newXaxisLayout.type,
                 'xaxis.rangebreaks': newXaxisLayout.rangebreaks,
                 'xaxis.tickangle': newXaxisLayout.tickangle // Dodaj inne potrzebne atrybuty osi X
                 // 'xaxis.nticks': newXaxisLayout.nticks // Jeśli używasz
            };
            Plotly.relayout(chartElement, layoutUpdate);
			
			// Zapisz nowy typ
            this.currentChartType = newType; 
            console.log("Typ wykresu zaktualizowany.");
        } catch (error) {
            console.error("Błąd podczas aktualizacji typu wykresu (Plotly.restyle):", error);
            // W razie błędu można spróbować przerysować cały wykres
            // this.drawChart(this.chartDivId, this.currentChartData, this.currentStockInfo);
        }
    },

    // Prywatne metody _create... zostały usunięte, bo ich rolę przejęły
    // funkcje z chartStyleConfig.js

    clearChart(chartDivId) {
         if (!chartDivId) return;
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
};

export { chartService };
