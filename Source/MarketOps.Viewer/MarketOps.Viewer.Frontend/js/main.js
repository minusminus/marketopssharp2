// js/main.js
import { uiHandler } from './uiHandler.js';
import { apiService } from './apiService.js';
import { chartService } from './chartService.js';

// Czekaj na załadowanie całej struktury DOM
document.addEventListener('DOMContentLoaded', () => {

    // --- Główna Logika Aplikacji (Orkiestracja) ---
    async function loadAndDisplayChart(params) {
        uiHandler.showLoading(true);
        uiHandler.toggleChartOptions(false);

        try {
            const apiData = await apiService.fetchStockData(
                params.stockId, params.timeframe, params.startDate, params.endDate
            );

            const selectedStockInfo = uiHandler.getSelectedStockInfo();

            // Spróbuj narysować wykres i sprawdź wynik
            const chartDrawnSuccessfully = chartService.drawChart(
                uiHandler.elements.plotlyChartDiv.id, // Przekaż ID diva z uiHandler
                apiData,
                selectedStockInfo
            );

            if (chartDrawnSuccessfully) {
                uiHandler.toggleChartOptions(true); // Pokaż opcje, jeśli rysowanie się powiodło
            } else {
                // Błąd został już prawdopodobnie zalogowany w chartService
                // Można opcjonalnie pokazać go też w UI
                uiHandler.showError("Wystąpił błąd podczas rysowania wykresu.");
            }

        } catch (error) {
            // Błędy z apiService lub inne nieprzechwycone
            uiHandler.showError(`Błąd podczas ładowania danych: ${error.message}`);
        } finally {
            uiHandler.showLoading(false);
        }
    }

    // --- Inicjalizacja UI ---
    // Przekaż główną funkcję orkiestracji jako callback do uiHandler
    uiHandler.init(loadAndDisplayChart);

}); // Koniec DOMContentLoaded