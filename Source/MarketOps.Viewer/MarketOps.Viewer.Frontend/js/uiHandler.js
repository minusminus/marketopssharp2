// js/uiHandler.js
import { apiService } from './apiService.js';
import { chartService } from './chartService.js';

const uiHandler = {
    elements: { /* ... (bez zmian - jak w poprzedniej wersji) ... */
        stockSelect: document.getElementById('stockSelect'),
        startDateInput: document.getElementById('startDate'),
        endDateInput: document.getElementById('endDate'),
        timeframeSelect: document.getElementById('timeframeSelect'),
        showChartButton: document.getElementById('showChartButton'),
        chartContainer: document.getElementById('chartContainer'),
        plotlyChartDiv: document.getElementById('plotlyChartDiv'),
        chartOptionsDiv: document.getElementById('chartOptions'),
        loadingIndicator: document.getElementById('loadingIndicator'),
        errorDisplay: document.getElementById('errorDisplay'),
        chartTypeCandleRadio: document.getElementById('chartTypeCandle'),
        chartTypeLineRadio: document.getElementById('chartTypeLine')
    },
    // Przechowujemy referencję do callbacku
    showChartCallback: null,

    init(showChartCallback) {
        this.showChartCallback = showChartCallback; // Zapisz callback
        this.setDefaultDates();
        this.setupEventListeners(); // Nie przekazujemy już callbacku tutaj
        this.loadStocks();
    },

    setDefaultDates() { /* ... (bez zmian) ... */
        const today = new Date();
        const oneYearAgo = new Date();
        oneYearAgo.setFullYear(today.getFullYear() - 1);
        this.elements.endDateInput.value = today.toISOString().split('T')[0];
        this.elements.startDateInput.value = oneYearAgo.toISOString().split('T')[0];
    },

    setupEventListeners() {
		// Listener dla przycisku "Pokaż Wykres"
        this.elements.showChartButton.addEventListener('click', () => {
            this.clearError();
            const params = this.getChartRequestParams();
            if (params.stockId && params.startDate && params.endDate) {
                // Wywołaj zapisany callback, jeśli istnieje
                if (this.showChartCallback) {
                    this.showChartCallback(params);
                } else {
                    console.error("showChartCallback nie został ustawiony w uiHandler.");
                }
            } else {
                if (!params.stockId) this.showError("Proszę wybrać instrument.");
                else if (!params.startDate || !params.endDate) this.showError("Proszę wybrać zakres dat.");
            }
        });
        // Listener dla zmiany typu wykresu
        this.elements.chartTypeCandleRadio.addEventListener('change', () => {
            if (this.elements.chartTypeCandleRadio.checked) {
                chartService.updateChartType('candlestick');
            }
        });
        this.elements.chartTypeLineRadio.addEventListener('change', () => {
            if (this.elements.chartTypeLineRadio.checked) {
                // Użyjemy 'scatter' z mode 'lines' dla wykresu liniowego
                chartService.updateChartType('scatter');
            }
        });
    },

    async loadStocks() {
        this.elements.stockSelect.innerHTML = '<option value="">Ładowanie...</option>'; // Pokaż ładowanie w select
        try {
            // Użyj zaimportowanego apiService
            const stocks = await apiService.fetchStocks();
            this.populateStockSelect(stocks);
        } catch (error) {
            console.error('Nie udało się pobrać listy spółek (uiHandler):', error);
            this.elements.stockSelect.innerHTML = '<option value="">Błąd ładowania</option>';
            this.showError(`Nie udało się pobrać listy spółek: ${error.message}`);
        }
    },

    populateStockSelect(stocks) { /* ... (bez zmian) ... */
        const select = this.elements.stockSelect;
        select.innerHTML = '<option value="">-- Wybierz instrument --</option>'; // Wyczyść
        stocks.forEach(stock => {
            const option = document.createElement('option');
            option.value = stock.id;
            option.dataset.symbol = stock.symbol;
            option.textContent = `${stock.symbol} (${stock.stockName})`;
            select.appendChild(option);
        });
    },

    getChartRequestParams() { /* ... (bez zmian) ... */
        return {
            stockId: this.elements.stockSelect.value,
            startDate: this.elements.startDateInput.value,
            endDate: this.elements.endDateInput.value,
            timeframe: this.elements.timeframeSelect.value
        };
    },

     getSelectedStockInfo() { /* ... (bez zmian) ... */
         const selectedOption = this.elements.stockSelect.options[this.elements.stockSelect.selectedIndex];
         if (!selectedOption || !selectedOption.value) {
             return null;
         }
         return {
             id: selectedOption.value,
             symbol: selectedOption.dataset.symbol,
             displayText: selectedOption.textContent
         };
     },

    showLoading(isLoading) { /* ... (bez zmian) ... */
        this.elements.loadingIndicator.style.display = isLoading ? 'block' : 'none';
    },

    clearError() { /* ... (bez zmian) ... */
        this.elements.errorDisplay.textContent = '';
    },

    showError(message) {
        console.error(message);
        this.elements.errorDisplay.textContent = message;
        // Użyj zaimportowanego chartService
        chartService.clearChart(this.elements.plotlyChartDiv.id);
        this.toggleChartOptions(false);
    },

    resetChartOptions() {
        this.elements.chartTypeCandleRadio.checked = true; // Domyślnie świecowy
        // Odznacz wskaźniki (dodamy później)
        // this.elements.indicatorSMACheckbox.checked = false;
    },

    toggleChartOptions(show) {
         this.elements.chartOptionsDiv.style.display = show ? 'block' : 'none';
         if (!show) {
            this.resetChartOptions(); // Resetuj opcje, gdy są ukrywane
         }
    }
};

export { uiHandler };
