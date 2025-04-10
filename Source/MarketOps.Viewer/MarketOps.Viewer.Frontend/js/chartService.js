// js/chartService.js

const chartService = {
    currentChartData: null,
    currentStockInfo: null,
    chartDivId: null,

    drawChart(chartDivId, apiData, stockInfo) {
        this.currentChartData = apiData;
        this.currentStockInfo = stockInfo;
        this.chartDivId = chartDivId;

        if (!apiData || apiData.length === 0) {
            console.error("Brak danych do narysowania wykresu.");
            this.clearChart(chartDivId); // Wyczyść na wszelki wypadek
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

            const candlestickTrace = {
                x: dates, open: opens, high: highs, low: lows, close: closes,
                type: 'candlestick', name: stockInfo?.symbol || 'Cena',
                yaxis: 'y',
                increasing: { line: { color: 'black', width: 1 }, fillcolor: 'white' },
                decreasing: { line: { color: 'black', width: 1 }, fillcolor: 'black' }
            };

            const volumeTrace = {
                x: dates, y: volumes, type: 'bar', name: 'Wolumen', yaxis: 'y2',
                marker: { color: 'rgba(100, 100, 255, 0.7)' }
            };

            const layout = {
                title: `Wykres dla ${stockInfo?.displayText || 'Wybrany instrument'}`,
                xaxis: {
                    rangeslider: { visible: false }, type: 'category',
                    nticks: 10
                    // Można dodać dynamiczne generowanie rangebreaks dla świąt jeśli trzeba
                },
                yaxis: { title: 'Cena', domain: [0.25, 1] },
                yaxis2: { title: 'Wolumen', domain: [0, 0.2], showticklabels: true, side: 'right' },
                hovermode: 'x unified', dragmode: 'zoom',
                legend: { x: 0, y: 1.1, orientation: "h" }
            };

            const chartElement = document.getElementById(chartDivId);
            if (!chartElement) {
                console.error(`Element o ID '${chartDivId}' nie został znaleziony do rysowania.`);
                return false;
            }

            // Używamy globalnego Plotly (dostarczonego przez CDN)
            Plotly.newPlot(chartElement, [candlestickTrace, volumeTrace], layout, { responsive: true });
            console.log("Wykres narysowany.");
            return true; // Sukces
        } catch(error) {
            console.error("Błąd podczas przygotowywania lub rysowania wykresu Plotly:", error);
            this.clearChart(chartDivId); // Spróbuj wyczyścić w razie błędu
            return false; // Zasygnalizuj błąd
        }
    },

    clearChart(chartDivId) {
        if (!chartDivId) return; // Sprawdź czy mamy ID
        const chartElement = document.getElementById(chartDivId);
        if (chartElement) {
            try {
                // Używamy globalnego Plotly
                Plotly.purge(chartElement);
                console.log(`Wykres w divie #${chartDivId} wyczyszczony.`);
            } catch (error) {
                // Czasami purge może rzucić błąd, jeśli div jest pusty; ignorujemy go
                 console.warn(`Problem podczas czyszczenia wykresu #${chartDivId}: ${error.message}`);
                 chartElement.innerHTML = ''; // Alternatywne czyszczenie
            }
        }
        this.currentChartData = null;
        this.currentStockInfo = null;
        this.chartDivId = null;
    }
};

export { chartService };
