// js/globalControls.js
import { apiService } from './apiService.js'; // Potrzebne do ładowania spółek

const globalControls = {
    elements: {
        stockSelect: document.getElementById('stockSelect'),
        startDateInput: document.getElementById('startDate'),
        endDateInput: document.getElementById('endDate'),
        timeframeSelect: document.getElementById('timeframeSelect'),
        addChartTabButton: document.getElementById('addChartTabButton'),
        // Można tu dodać referencję do globalnego kontenera błędów, jeśli taki powstanie
    },
    addChartTabCallback: null, // Callback wywoływany po kliknięciu przycisku

    init(addChartTabCallback) {
        this.addChartTabCallback = addChartTabCallback;
        this._setDefaultDates();
        this._setupEventListeners();
        this.loadStocks();
    },

    _setDefaultDates() {
        const today = new Date();
        const oneYearAgo = new Date();
        oneYearAgo.setFullYear(today.getFullYear() - 1);
        this.elements.endDateInput.value = today.toISOString().split('T')[0];
        this.elements.startDateInput.value = oneYearAgo.toISOString().split('T')[0];
    },

    _setupEventListeners() {
        this.elements.addChartTabButton.addEventListener('click', () => {
            // Usuń globalny błąd, jeśli istnieje
            // this.clearGlobalError();
            const params = this.getChartRequestParams();
            const stockInfo = this.getSelectedStockInfo(); // Pobierz też info o spółce

            if (params.stockId && params.startDate && params.endDate && stockInfo) {
                if (this.addChartTabCallback) {
                    // Przekaż zarówno parametry zapytania, jak i info o spółce
                    this.addChartTabCallback(params, stockInfo);
                }
            } else {
                 this.showGlobalError("Proszę wybrać instrument, daty i upewnić się, że lista instrumentów jest załadowana.");
            }
        });
    },

    async loadStocks() {
         this.elements.stockSelect.innerHTML = '<option value="">Ładowanie...</option>';
        try {
            const stocks = await apiService.fetchStocks();
            this._populateStockSelect(stocks);
        } catch (error) {
            console.error('Nie udało się pobrać listy spółek (globalControls):', error);
            this.elements.stockSelect.innerHTML = '<option value="">Błąd ładowania</option>';
            this.showGlobalError(`Nie udało się pobrać listy spółek: ${error.message}`);
        }
    },

    _populateStockSelect(stocks) {
        const select = this.elements.stockSelect;
        select.innerHTML = '<option value="">-- Wybierz instrument --</option>';
        stocks.forEach(stock => {
            const option = document.createElement('option');
            option.value = stock.id;
            option.dataset.symbol = stock.symbol;
             // Zapiszmy też pełną nazwę w data-*
            option.dataset.displayText = `${stock.symbol} (${stock.stockName})`;
            option.textContent = option.dataset.displayText;
            select.appendChild(option);
        });
    },

    getChartRequestParams() {
        return {
            stockId: this.elements.stockSelect.value,
            startDate: this.elements.startDateInput.value,
            endDate: this.elements.endDateInput.value,
            timeframe: this.elements.timeframeSelect.value
        };
    },

    getSelectedStockInfo() {
         const selectedOption = this.elements.stockSelect.options[this.elements.stockSelect.selectedIndex];
         if (!selectedOption || !selectedOption.value) {
             return null;
         }
         return {
             id: selectedOption.value,
             symbol: selectedOption.dataset.symbol,
             // Pobierz pełny tekst z atrybutu data-*
             displayText: selectedOption.dataset.displayText
         };
     },

     // TODO: Zaimplementować lepsze globalne błędy
     showGlobalError(message) {
         console.error("Global Error:", message);
         alert(message);
     }
     /* clearGlobalError() { ... } */
};

export { globalControls };