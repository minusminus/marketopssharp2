// js/uiHandler.js
import { apiService } from './apiService.js';
import { chartService } from './chartService.js';

const uiHandler = {
    elements: {
        stockSelect: document.getElementById('stockSelect'),
        startDateInput: document.getElementById('startDate'),
        endDateInput: document.getElementById('endDate'),
        timeframeSelect: document.getElementById('timeframeSelect'),
        addChartTabButton: document.getElementById('addChartTabButton'),
        tabButtonsContainer: document.getElementById('tabButtons'),
        tabContentContainer: document.getElementById('tabContent'),
        noTabsMessage: document.getElementById('noTabsMessage'),
    },
    addChartTabCallback: null, // Zmieniamy nazwę callbacku
    closeTabCallback: null,    // Callback do powiadomienia logiki o zamknięciu
    switchTabCallback: null,   // Callback do powiadomienia logiki o zmianie aktywnej zakładki
    activeTabId: null,
    tabCounter: 0, // Licznik do generowania unikalnych ID

    init(addChartTabCallback, closeTabCallback, switchTabCallback) {
        this.addChartTabCallback = addChartTabCallback;
        this.closeTabCallback = closeTabCallback;
        this.switchTabCallback = switchTabCallback;
        this.setDefaultDates();
        this.setupEventListeners();
        this.loadStocks(); // Rozpoczyna ładowanie spółek (bez zmian)
        this._updateNoTabsMessage(); // Pokaż lub ukryj wiadomość początkową
    },

    setDefaultDates() {
        const today = new Date();
        const oneYearAgo = new Date();
        oneYearAgo.setFullYear(today.getFullYear() - 1);
        this.elements.endDateInput.value = today.toISOString().split('T')[0];
        this.elements.startDateInput.value = oneYearAgo.toISOString().split('T')[0];
    },

    setupEventListeners() {
        // Listener dla przycisku "Pokaż/Dodaj Wykres"
        this.elements.addChartTabButton.addEventListener('click', () => {
            this.clearGlobalError(); // Czyści ewentualne globalne błędy (jeśli dodamy)
            const params = this.getChartRequestParams();
            if (params.stockId && params.startDate && params.endDate) {
                if (this.addChartTabCallback) {
                    // Callback powinien teraz utworzyć zakładkę i załadować dane
                    this.addChartTabCallback(params);
                }
            } else {
                if (!params.stockId) this.showGlobalError("Proszę wybrać instrument.");
                else if (!params.startDate || !params.endDate) this.showGlobalError("Proszę wybrać zakres dat.");
            }
        });

        // Listener do przełączania zakładek (delegacja zdarzeń)
        this.elements.tabButtonsContainer.addEventListener('click', (event) => {
            const target = event.target;
            // Kliknięcie na przycisk zakładki (ale nie na 'x')
            if (target.classList.contains('tab-button') && !target.classList.contains('close-tab-btn')) {
                const tabId = target.dataset.tabId;
                if (tabId && tabId !== this.activeTabId) {
                    this.switchTab(tabId);
                }
            }
            // Kliknięcie na przycisk zamykania 'x'
            else if (target.classList.contains('close-tab-btn')) {
                 event.stopPropagation(); // Zapobiegaj przełączeniu zakładki przy kliknięciu 'x'
                const tabId = target.dataset.tabId;
                this.closeTab(tabId);
            }
        });
	},
	
    // Funkcja dodająca nową zakładkę (wywoływana przez główną logikę)
    addTab(tabId) {
        // 1. Stwórz przycisk zakładki
        const button = document.createElement('button');
        button.className = 'tab-button';
        button.dataset.tabId = tabId;

        button.dataset.stockId = '';
        button.dataset.symbol = '';
        button.dataset.timeframe = '';

        const closeBtn = document.createElement('span');
        closeBtn.className = 'close-tab-btn';
        closeBtn.dataset.tabId = tabId;
        closeBtn.innerHTML = '×'; // Znak 'x'
        button.appendChild(closeBtn);

        this.elements.tabButtonsContainer.appendChild(button);

        // 2. Stwórz panel zawartości zakładki
        const pane = document.createElement('div');
        pane.id = tabId;
        pane.className = 'tab-pane';
        const plotlyDivId = `plotly-chart-${tabId}`; // Użyj tej samej konwencji co chartService
		
        // Dodaj wewnętrzną strukturę dla opcji, wykresu, loadingu, błędów
        pane.innerHTML = `
            <div class="chart-options" style="display: none;">
                 <div class="option-group">
                    <span class="option-label">Typ wykresu:</span>
                    <input type="radio" id="chartTypeCandle_${tabId}" name="chartType_${tabId}" value="candle" checked> <label for="chartTypeCandle_${tabId}">Świecowy</label>
                    <input type="radio" id="chartTypeLine_${tabId}" name="chartType_${tabId}" value="line"> <label for="chartTypeLine_${tabId}">Liniowy</label>
                 </div>
                 <div class="option-group">
                    <span class="option-label">Wskaźniki:</span>
                    <input type="checkbox" id="indicatorSMA_${tabId}"> <label for="indicatorSMA_${tabId}">SMA(20)</label>
                    <input type="checkbox" id="indicatorChannel_${tabId}"> <label for="indicatorChannel_${tabId}">Kanał Ceny(20)</label>
                    <input type="checkbox" id="indicatorBB_${tabId}"> <label for="indicatorBB_${tabId}">Bollinger(20, 2)</label>
                    <input type="checkbox" id="indicatorATR_${tabId}"> <label for="indicatorATR_${tabId}">ATR(14)</label>
                 </div>
            </div>
            <div id="${plotlyDivId}" class="plotly-chart-div"> <!-- Zagnieżdżony div dla Plotly -->
                <div class="loading-indicator" style="display: none;">
                     <div class="spinner"></div>
                     Ładowanie danych...
                 </div>
            </div>
            <div class="error-display"></div>
        `;
        this.elements.tabContentContainer.appendChild(pane);

        this._setupTabOptionListeners(tabId);
        this.switchTab(tabId);
        this._updateNoTabsMessage();
		
		return button;
    },
	
    setTabButtonContent(button, tabId, stockInfo, timeframe) {
        if (!button || !stockInfo) return;

        const tabTitle = `${stockInfo.symbol} (${this._getTimeframeDisplayName(timeframe)})`;
        const tooltip = `${stockInfo.displayText} - ${this._getTimeframeDisplayName(timeframe)}`;

        // Ustaw tekst i tooltip
        // Usuń stary tekst (jeśli był) i przycisk 'x', potem dodaj nowy tekst i 'x'
        const closeBtn = button.querySelector('.close-tab-btn'); // Zachowaj referencję do 'x'
        button.textContent = ''; // Wyczyść
        button.appendChild(document.createTextNode(tabTitle)); // Dodaj nowy tekst
        if (closeBtn) button.appendChild(closeBtn); // Dodaj 'x' z powrotem

        button.title = tooltip;

        // Zaktualizuj atrybuty data-*
        button.dataset.stockId = stockInfo.id;
        button.dataset.symbol = stockInfo.symbol;
        button.dataset.timeframe = timeframe;
    },

     _getTimeframeDisplayName(timeframeKey) {
         switch (timeframeKey) {
             case 'daily': return 'Dzienny';
             case 'weekly': return 'Tygodniowy';
             case 'monthly': return 'Miesięczny';
             default: return timeframeKey;
         }
     },

    _setupTabOptionListeners(tabId) {
        const pane = document.getElementById(tabId);
        if (!pane) return;

        const candleRadio = pane.querySelector(`#chartTypeCandle_${tabId}`);
        const lineRadio = pane.querySelector(`#chartTypeLine_${tabId}`);
        const smaCheck = pane.querySelector(`#indicatorSMA_${tabId}`);
        const channelCheck = pane.querySelector(`#indicatorChannel_${tabId}`);
        const bbCheck = pane.querySelector(`#indicatorBB_${tabId}`);
        const atrCheck = pane.querySelector(`#indicatorATR_${tabId}`);

        candleRadio?.addEventListener('change', () => {
            if (candleRadio.checked) chartService.updateChartType(tabId, 'candlestick'); // Przekaż tabId
        });
        lineRadio?.addEventListener('change', () => {
            if (lineRadio.checked) chartService.updateChartType(tabId, 'scatter'); // Przekaż tabId
        });
        smaCheck?.addEventListener('change', (e) => chartService.toggleIndicator(tabId, 'SMA', e.target.checked)); // Przekaż tabId
        channelCheck?.addEventListener('change', (e) => chartService.toggleIndicator(tabId, 'Channel', e.target.checked)); // Przekaż tabId
        bbCheck?.addEventListener('change', (e) => chartService.toggleIndicator(tabId, 'BB', e.target.checked)); // Przekaż tabId
        atrCheck?.addEventListener('change', (e) => chartService.toggleIndicator(tabId, 'ATR', e.target.checked)); // Przekaż tabId
    },

    switchTab(tabId) {
        if (!tabId) return;
		if (tabId === this.activeTabId) return;
		
        console.log("Switching to tab:", tabId);
        // Dezaktywuj poprzednią zakładkę
        if (this.activeTabId) {
            const oldButton = this.elements.tabButtonsContainer.querySelector(`.tab-button[data-tab-id="${this.activeTabId}"]`);
            const oldPane = document.getElementById(this.activeTabId);
            oldButton?.classList.remove('active');
            oldPane?.classList.remove('active');
        }

        // Aktywuj nową zakładkę
        const newButton = this.elements.tabButtonsContainer.querySelector(`.tab-button[data-tab-id="${tabId}"]`);
        const newPane = document.getElementById(tabId);
        newButton?.classList.add('active');
        newPane?.classList.add('active');

        this.activeTabId = tabId;

        // Wywołaj resize dla wykresu w aktywowanej zakładce
        // Upewnij się, że robisz to PO tym, jak panel stał się widoczny (zmiana display na block)
        // Czasami potrzebne jest małe opóźnienie, aby przeglądarka przeliczyła wymiary
        setTimeout(() => {
            const plotlyDivId = `plotly-chart-${tabId}`; // Znajdź ID diva wykresu
            const chartElement = document.getElementById(plotlyDivId);
            if (chartElement && typeof Plotly !== 'undefined') {
                try {
                     // Sprawdź, czy wykres Plotly został już zainicjalizowany w tym divie
                     // (graphDiv._context jest jednym ze sposobów, ale może być nieoficjalny)
                     if (chartElement.layout) { // Lepsze sprawdzenie: czy ma obiekt layout (nadany przez newPlot)
                         console.log(`Resizing Plotly chart: ${plotlyDivId}`);
                         Plotly.Plots.resize(chartElement);
                     } else {
                         console.log(`Plotly chart ${plotlyDivId} not yet initialized, skipping resize.`);
                     }
                } catch (error) {
                    console.error(`Error resizing Plotly chart ${plotlyDivId}:`, error);
                }
            }
        }, 0); // setTimeout z 0ms opóźnienia daje przeglądarce chwilę na renderowanie
        // ================================================================

        // Powiadom główną logikę o zmianie (jeśli potrzebne)
        if (this.switchTabCallback) {
             this.switchTabCallback(tabId);
        }
    },

    closeTab(tabId) {
         console.log("Closing tab:", tabId);
        const button = this.elements.tabButtonsContainer.querySelector(`.tab-button[data-tab-id="${tabId}"]`);
        const pane = document.getElementById(tabId);

        if (!button || !pane) return;

        // Wywołaj callback, aby logika mogła wyczyścić dane wykresu
        if (this.closeTabCallback) {
            this.closeTabCallback(tabId); // chartService.clearChart zostanie wywołane w logice głównej
        }

        // Usuń elementy z DOM
        button.remove();
        pane.remove();

        // Jeśli zamykana zakładka była aktywna, aktywuj inną (np. ostatnią lub pierwszą)
        if (this.activeTabId === tabId) {
            const remainingButtons = this.elements.tabButtonsContainer.querySelectorAll('.tab-button');
            if (remainingButtons.length > 0) {
                this.switchTab(remainingButtons[remainingButtons.length - 1].dataset.tabId); // Aktywuj ostatnią
            } else {
                this.activeTabId = null; // Brak aktywnych zakładek
            }
        }
         this._updateNoTabsMessage(); // Pokaż/ukryj wiadomość
    },

     // Pokazuje/ukrywa wskaźnik ładowania w AKTYWNEJ zakładce
    showLoading(isLoading, tabId = this.activeTabId) {
        if (!this.activeTabId) return;
        if (!tabId) return;
        const pane = document.getElementById(tabId);
        const indicator = pane?.querySelector('.loading-indicator');
        if (indicator) {
            // Upewnij się, że display jest 'flex' przy pokazywaniu
            indicator.style.display = isLoading ? 'flex' : 'none';
        } else if (isLoading) {
             // Loguj, jeśli próbujemy pokazać loading, a elementu nie ma
             console.warn(`Nie znaleziono wskaźnika ładowania dla zakładki ${tabId}`);
        }
    },

     // Pokazuje/ukrywa opcje w AKTYWNEJ zakładce
    toggleChartOptions(show, tabId = this.activeTabId) {
         if (!tabId) return;
         const pane = document.getElementById(tabId);
         const options = pane?.querySelector('.chart-options');
         if (options) {
              options.style.display = show ? 'block' : 'none';
              // Resetowanie opcji dla konkretnej zakładki jest nadal pominięte
              // if (!show) this.resetChartOptions(tabId);
         }
    },

    // Wyświetla błąd w AKTYWNEJ zakładce
    showError(message, tabId = this.activeTabId) {
        if (!tabId) {
            console.error("Próba wyświetlenia błędu bez aktywnej zakładki:", message);
            // Można dodać globalny kontener na błędy
            return;
        }
        const pane = document.getElementById(tabId);
        const errorDisplay = pane?.querySelector('.error-display');
        if (errorDisplay) {
            errorDisplay.textContent = message;
        }
        // Wyczyść też wykres w tej zakładce
        chartService.clearChart(tabId); // Wywołaj czyszczenie dla ID ZAKŁADKI
        this.toggleChartOptions(false); // Ukryj opcje
    },

    // Czyści błąd w AKTYWNEJ zakładce
    clearError(tabId = this.activeTabId) {
        if (!tabId) return;
        const pane = document.getElementById(tabId);
        const errorDisplay = pane?.querySelector('.error-display');
        if (errorDisplay) {
            errorDisplay.textContent = '';
        }
    },

     // Czyści błąd globalny (jeśli zaimplementowany)
    clearGlobalError() {
        // TODO: zaimplementować, jeśli potrzebne
    },
    // Wyświetla błąd globalny (jeśli zaimplementowany)
    showGlobalError(message){
         console.error("Global Error:", message);
         alert(message); // Prosty alert jako fallback
         // TODO: zaimplementować lepszy mechanizm
    },

    // Aktualizuje widoczność wiadomości "brak zakładek"
    _updateNoTabsMessage() {
        const hasTabs = this.elements.tabButtonsContainer.querySelector('.tab-button');
        this.elements.noTabsMessage.style.display = hasTabs ? 'none' : 'block';
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

    // resetChartOptions() {
        // this.elements.chartTypeCandleRadio.checked = true; // Domyślnie świecowy
        // this.elements.indicatorSMACheckbox.checked = false;
        // this.elements.indicatorChannelCheckbox.checked = false;
        // this.elements.indicatorBBCheckbox.checked = false;
        // this.elements.indicatorATRCheckbox.checked = false;
    // },
};

export { uiHandler };
