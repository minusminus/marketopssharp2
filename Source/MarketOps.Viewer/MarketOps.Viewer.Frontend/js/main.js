// js/main.js
import { uiHandler } from './uiHandler.js';
import { apiService } from './apiService.js';
import { chartService } from './chartService.js';

// Czekaj na załadowanie całej struktury DOM
document.addEventListener('DOMContentLoaded', () => {

	let tabCounter = 0; // Przeniesiono licznik tutaj

    // --- Główna Logika Aplikacji (Orkiestracja) ---
    async function handleAddOrShowChart(params) {
        // uiHandler.showLoading(true, uiHandler.activeTabId);
		
		const newTabId = `chart-tab-${++tabCounter}`; // Inkrementuj i przypisz
		let currentTabActivated = false; // Flaga śledząca, czy zakładka została już dodana do DOM
		let newTabButton = null; // Przechowamy referencję do przycisku

        try {
            // Pobierz dane z API
            const apiDataPromise = apiService.fetchStockData(
                params.stockId, params.timeframe, params.startDate, params.endDate
            );
            const selectedStockInfo = uiHandler.getSelectedStockInfo();
            if (!selectedStockInfo) throw new Error("Nie udało się pobrać informacji o instrumencie.");

            // Dodaj zakładkę - to ją aktywuje
            newTabButton = uiHandler.addTab(newTabId, selectedStockInfo.symbol);
			currentTabActivated = true; // Zakładka istnieje w DOM
			console.log('timeframe: ' + params.timeframe);
			uiHandler.setTabButtonContent(newTabButton, newTabId, selectedStockInfo, params.timeframe);
            // Pokaż loading w NOWEJ zakładce
            uiHandler.showLoading(true, newTabId);
            // Ukryj opcje w nowej zakładce
            uiHandler.toggleChartOptions(false, newTabId);
            // Wyczyść błędy w nowej zakładce
            uiHandler.clearError(newTabId);

            // Czekaj na zakończenie pobierania danych
            const apiData = await apiDataPromise;

             // Narysuj wykres w NOWEJ (aktywnej) zakładce
             const chartDrawn = chartService.drawChart(newTabId, apiData, selectedStockInfo);
			uiHandler.showLoading(false, newTabId);

             if (chartDrawn) {
                  uiHandler.toggleChartOptions(true, newTabId); // Pokaż opcje w aktywnej zakładce
             } else {
                  uiHandler.showError("Błąd podczas rysowania wykresu.", newTabId);
             }

        } catch (error) {
             console.error(`Błąd w handleAddOrShowChart dla ${newTabId}:`, error);
             // Jeśli zakładka została już dodana, pokaż błąd w niej
             if (currentTabActivated) {
                 uiHandler.showError(`Błąd: ${error.message}`, newTabId);
                 // Ukryj loading w tej zakładce, jeśli błąd wystąpił przed ukryciem
                 uiHandler.showLoading(false, newTabId);
                 uiHandler.toggleChartOptions(false, newTabId); // Ukryj też opcje
             } else {
                 uiHandler.showGlobalError(`Błąd: ${error.message}`);
             }
        }
    }

    // Funkcja wywoływana przez UI po zamknięciu zakładki
    function handleCloseTab(tabId) {
        chartService.clearChart(tabId); // Powiadom serwis wykresów, aby wyczyścił stan
    }

     // Funkcja wywoływana przez UI po zmianie aktywnej zakładki (na razie pusta, może być potrzebna później)
     function handleSwitchTab(tabId) {
         console.log(`Aktywowano zakładkę: ${tabId}`);
         // Można tu np. odświeżyć dane lub dostosować UI globalne
     }


    // --- Inicjalizacja UI ---
    // Przekaż funkcje obsługi zdarzeń do uiHandler
    uiHandler.init(handleAddOrShowChart, handleCloseTab, handleSwitchTab);

}); // Koniec DOMContentLoaded