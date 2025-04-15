// js/tabManager.js
import { chartService } from './chartService.js'; // Potrzebny do powiadamiania o zmianach

const tabManager = {
    elements: {
        tabButtonsContainer: document.getElementById('tabButtons'),
        tabContentContainer: document.getElementById('tabContent'),
        noTabsMessage: document.getElementById('noTabsMessage'),
    },
    closeTabCallback: null,    // Callback do main.js przy zamykaniu
    switchTabCallback: null,   // Callback do main.js przy przełączaniu
    activeTabId: null,

    init(closeTabCallback, switchTabCallback) {
        this.closeTabCallback = closeTabCallback;
        this.switchTabCallback = switchTabCallback;
        this._setupEventListeners();
        this._updateNoTabsMessage();
    },

    _setupEventListeners() {
        // Listener do przełączania i zamykania zakładek (delegacja)
        this.elements.tabButtonsContainer.addEventListener('click', (event) => {
            const target = event.target;
            if (target.classList.contains('tab-button') && !target.classList.contains('close-tab-btn')) {
                const tabId = target.dataset.tabId;
                if (tabId && tabId !== this.activeTabId) {
                    this.switchTab(tabId);
                }
            }
            else if (target.classList.contains('close-tab-btn')) {
                 event.stopPropagation();
                const tabId = target.dataset.tabId;
                this.closeTab(tabId);
            }
        });
    },

    // Tworzy strukturę DOM zakładki i zwraca ID diva dla Plotly
    addTab(tabId) {
        // 1. Stwórz przycisk (pusty)
        const button = document.createElement('button');
        button.className = 'tab-button';
        button.dataset.tabId = tabId;
        button.dataset.stockId = ''; // Inicjalizuj puste
        button.dataset.symbol = '';
        button.dataset.timeframe = '';

        const closeBtn = document.createElement('span');
        closeBtn.className = 'close-tab-btn';
        closeBtn.dataset.tabId = tabId;
        closeBtn.innerHTML = '×';
        button.appendChild(closeBtn);
        this.elements.tabButtonsContainer.appendChild(button);

        // 2. Stwórz panel
        const pane = document.createElement('div');
        pane.id = tabId;
        pane.className = 'tab-pane';
        const plotlyDivId = `plotly-chart-${tabId}`;
        pane.innerHTML = `
            <div class="chart-options" style="display: none;">
                 <div class="option-group">
                    <span class="option-label">Typ:</span>
                    <input type="radio" id="chartTypeCandle_${tabId}" name="chartType_${tabId}" value="candle" checked> <label for="chartTypeCandle_${tabId}">Ś</label>
                    <input type="radio" id="chartTypeLine_${tabId}" name="chartType_${tabId}" value="line"> <label for="chartTypeLine_${tabId}">L</label>
                 </div>
                 <div class="option-group">
                    <span class="option-label">Wsk:</span>
                    <input type="checkbox" id="indicatorSMA_${tabId}"> <label for="indicatorSMA_${tabId}">SMA</label>
                    <input type="checkbox" id="indicatorChannel_${tabId}"> <label for="indicatorChannel_${tabId}">Kan</label>
                    <input type="checkbox" id="indicatorBB_${tabId}"> <label for="indicatorBB_${tabId}">BB</label>
                    <input type="checkbox" id="indicatorATR_${tabId}"> <label for="indicatorATR_${tabId}">ATR</label>
                 </div>
            </div>
            <div id="${plotlyDivId}" class="plotly-chart-div">
                <div class="loading-indicator" style="display: none;">
					<div class="spinner"></div>
					Ładowanie danych...
				</div>
            </div>
            <div class="error-display"></div>
        `; // Skrócone etykiety dla oszczędności miejsca
        this.elements.tabContentContainer.appendChild(pane);

        this._setupTabOptionListeners(tabId, plotlyDivId); // Przekaż plotlyDivId

        this.switchTab(tabId); // Aktywuj nową
        this._updateNoTabsMessage();

        // Zwróć przycisk i ID diva plotly
        return { button, plotlyDivId };
    },

    _setupTabOptionListeners(tabId, plotlyDivId) { // Przyjmuje plotlyDivId
        const pane = document.getElementById(tabId);
        if (!pane) return;

        // Znajdź elementy wewnątrz panelu
        const candleRadio = pane.querySelector(`#chartTypeCandle_${tabId}`);
        const lineRadio = pane.querySelector(`#chartTypeLine_${tabId}`);
        const smaCheck = pane.querySelector(`#indicatorSMA_${tabId}`);
        const channelCheck = pane.querySelector(`#indicatorChannel_${tabId}`);
        const bbCheck = pane.querySelector(`#indicatorBB_${tabId}`);
        const atrCheck = pane.querySelector(`#indicatorATR_${tabId}`);

        // Przypisz listenery wywołujące chartService z ID zakładki
        candleRadio?.addEventListener('change', () => {
            if (candleRadio.checked) chartService.updateChartType(tabId, 'candlestick');
        });
        lineRadio?.addEventListener('change', () => {
            if (lineRadio.checked) chartService.updateChartType(tabId, 'scatter');
        });
        smaCheck?.addEventListener('change', (e) => chartService.toggleIndicator(tabId, 'SMA', e.target.checked));
        channelCheck?.addEventListener('change', (e) => chartService.toggleIndicator(tabId, 'Channel', e.target.checked));
        bbCheck?.addEventListener('change', (e) => chartService.toggleIndicator(tabId, 'BB', e.target.checked));
        atrCheck?.addEventListener('change', (e) => chartService.toggleIndicator(tabId, 'ATR', e.target.checked));
    },


    setTabButtonContent(button, tabId, stockInfo, timeframe) {
         if (!button || !stockInfo) return;
         const tabTitle = `${stockInfo.symbol} (${timeframe})`;
         const tooltip = `${stockInfo.displayText} - ${this._getTimeframeDisplayName(timeframe)}`;

         const closeBtn = button.querySelector('.close-tab-btn');
         button.textContent = '';
         button.appendChild(document.createTextNode(tabTitle));
         if (closeBtn) button.appendChild(closeBtn);
         button.title = tooltip;

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

    switchTab(tabId) {
        if (!tabId || tabId === this.activeTabId) return;

        // Dezaktywuj starą
        if (this.activeTabId) {
            const oldButton = this.elements.tabButtonsContainer.querySelector(`.tab-button[data-tab-id="${this.activeTabId}"]`);
            const oldPane = document.getElementById(this.activeTabId);
            oldButton?.classList.remove('active');
            oldPane?.classList.remove('active');
        }

        // Aktywuj nową
        const newButton = this.elements.tabButtonsContainer.querySelector(`.tab-button[data-tab-id="${tabId}"]`);
        const newPane = document.getElementById(tabId);
        newButton?.classList.add('active');
        newPane?.classList.add('active');

        this.activeTabId = tabId;
		this.showLoading(false, tabId); // Jawnie ukryj wskaźnik dla tej zakładki

        setTimeout(() => {
            const plotlyDivId = `plotly-chart-${tabId}`;
            const chartElement = document.getElementById(plotlyDivId);
            if (chartElement && typeof Plotly !== 'undefined' && chartElement.layout) {
                 try { Plotly.Plots.resize(chartElement); } catch (e) { console.error(e); }
            }
        }, 0);

        if (this.switchTabCallback) this.switchTabCallback(tabId);
    },

    closeTab(tabId) {
        const button = this.elements.tabButtonsContainer.querySelector(`.tab-button[data-tab-id="${tabId}"]`);
        const pane = document.getElementById(tabId);
        if (!button || !pane) return;

        // === ZMIANA: Powiadom main.js ZANIM usuniesz elementy ===
        if (this.closeTabCallback) {
            this.closeTabCallback(tabId); // main.js wywoła chartService.clearChart
        }
        // =======================================================

        const wasActive = (this.activeTabId === tabId);
        let nextActiveTabId = null;

         // Znajdź następną zakładkę do aktywacji (np. poprzednią lub następną)
         if (wasActive) {
            const prevButton = button.previousElementSibling;
            const nextButton = button.nextElementSibling;
            if (prevButton && prevButton.classList.contains('tab-button')) {
                nextActiveTabId = prevButton.dataset.tabId;
            } else if (nextButton && nextButton.classList.contains('tab-button')) {
                 nextActiveTabId = nextButton.dataset.tabId;
            } else {
                 const remaining = this.elements.tabButtonsContainer.querySelector('.tab-button:not([data-tab-id="' + tabId + '"])');
                 if (remaining) nextActiveTabId = remaining.dataset.tabId;
            }
         }

        button.remove();
        pane.remove();

        if (wasActive) {
            this.activeTabId = null; // Zresetuj aktywną przed przełączeniem
            if (nextActiveTabId) {
                 this.switchTab(nextActiveTabId); // Aktywuj inną
            }
        }
        this._updateNoTabsMessage();
    },

    showLoading(isLoading, tabId = this.activeTabId) {
        if (!tabId) return;
        const pane = document.getElementById(tabId);
        const indicator = pane?.querySelector('.loading-indicator');
        if (indicator) indicator.style.display = isLoading ? 'flex' : 'none';
    },

    toggleChartOptions(show, tabId = this.activeTabId) {
         if (!tabId) return;
         const pane = document.getElementById(tabId);
         const options = pane?.querySelector('.chart-options');
         if (options) options.style.display = show ? 'block' : 'none';
    },

    showError(message, tabId = this.activeTabId) {
        if (!tabId) { console.error("No active tab for error:", message); return; }
        const pane = document.getElementById(tabId);
        const errorDisplay = pane?.querySelector('.error-display');
        if (errorDisplay) errorDisplay.textContent = message;
        // Nie czyścimy tu wykresu, bo to robi chartService.clearChart wywołane przez main.js
        this.toggleChartOptions(false, tabId);
    },

    clearError(tabId = this.activeTabId) {
        if (!tabId) return;
        const pane = document.getElementById(tabId);
        const errorDisplay = pane?.querySelector('.error-display');
        if (errorDisplay) errorDisplay.textContent = '';
    },

    _updateNoTabsMessage() {
        const hasTabs = this.elements.tabButtonsContainer.querySelector('.tab-button');
        this.elements.noTabsMessage.style.display = hasTabs ? 'none' : 'block';
    }
};

export { tabManager };