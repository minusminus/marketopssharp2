// js/main.js
import { globalControls } from './globalControls.js'; // Nowy moduł
import { tabManager } from './tabManager.js';       // Nowy moduł
import { apiService } from './apiService.js';
import { chartService } from './chartService.js';

document.addEventListener('DOMContentLoaded', () => {

    let tabCounter = 0;

    // Funkcja wywoływana przez globalControls po kliknięciu przycisku
    async function handleAddOrShowChart(params, stockInfo) { // Otrzymuje oba zestawy danych
        const newTabId = `chart-tab-${++tabCounter}`;
        let tabElements = null; // Przechowa { button, plotlyDivId }

        tabManager.showLoading(true, tabManager.activeTabId); // Pokaż loading w aktywnej (jeśli jest)

        try {
            // Pobierz dane w tle
            const apiDataPromise = apiService.fetchStockData(
                params.stockId, params.timeframe, params.startDate, params.endDate
            );

            // Dodaj strukturę zakładki i pobierz referencje
            tabElements = tabManager.addTab(newTabId); // Zwraca { button, plotlyDivId }
            tabManager.setTabButtonContent(tabElements.button, newTabId, stockInfo, params.timeframe);

            // Pokaż loading w nowej zakładce
            tabManager.showLoading(true, newTabId);
            tabManager.toggleChartOptions(false, newTabId);
            tabManager.clearError(newTabId);

            // Czekaj na dane
            const apiData = await apiDataPromise;

            // Narysuj wykres
            // Przekazujemy ID ZAKŁADKI, a nie ID diva plotly
            const chartDrawn = chartService.drawChart(newTabId, apiData, stockInfo);

            // Ukryj loading
            tabManager.showLoading(false, newTabId);

            if (chartDrawn) {
                tabManager.toggleChartOptions(true, newTabId);
            } else {
                tabManager.showError("Błąd podczas rysowania wykresu.", newTabId);
            }

        } catch (error) {
            console.error(`Błąd w handleAddOrShowChart dla ${newTabId}:`, error);
             const targetTabId = tabElements ? newTabId : null; // Sprawdź, czy zakładka została utworzona
             if (targetTabId) {
                 tabManager.showError(`Błąd: ${error.message}`, targetTabId);
                 tabManager.showLoading(false, targetTabId); // Ukryj loading w razie błędu
                 tabManager.toggleChartOptions(false, targetTabId);
             } else {
                 globalControls.showGlobalError(`Błąd: ${error.message}`);
             }
        } finally {
             // Ukryj loading w poprzedniej aktywnej, jeśli się zmieniła (może nie być potrzebne)
             // if (tabManager.activeTabId !== newTabId) { ... }
        }
    }

    // Funkcja wywoływana przez tabManager po zamknięciu zakładki
    function handleCloseTab(tabId) {
        chartService.clearChart(tabId); // Powiadom serwis wykresów
    }

     // Funkcja wywoływana przez tabManager po zmianie aktywnej zakładki
     function handleSwitchTab(tabId) {
         // Na razie nic nie robimy, ale można np. zaktualizować tytuł strony
     }

    // --- Inicjalizacja Modułów ---
    globalControls.init(handleAddOrShowChart);
    tabManager.init(handleCloseTab, handleSwitchTab);

}); // Koniec DOMContentLoaded