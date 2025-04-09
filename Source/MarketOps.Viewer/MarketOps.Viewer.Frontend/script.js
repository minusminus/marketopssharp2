// Czekaj na załadowanie całej struktury DOM
document.addEventListener('DOMContentLoaded', () => {
    const stockSelect = document.getElementById('stockSelect');
    const startDateInput = document.getElementById('startDate');
    const endDateInput = document.getElementById('endDate');
    const timeframeSelect = document.getElementById('timeframeSelect');
    const showChartButton = document.getElementById('showChartButton');
    const chartContainer = document.getElementById('chartContainer'); // Główny kontener
    const plotlyChartDiv = document.getElementById('plotlyChartDiv'); // Kontener dla Plotly
    const chartOptionsDiv = document.getElementById('chartOptions');
    const loadingIndicator = document.getElementById('loadingIndicator');
    const errorDisplay = document.getElementById('errorDisplay');

    // --- Domyślne daty ---
    const today = new Date();
    const oneYearAgo = new Date();
    oneYearAgo.setFullYear(today.getFullYear() - 1);
    // Format YYYY-MM-DD wymagany przez input type="date"
    endDateInput.value = today.toISOString().split('T')[0];
    startDateInput.value = oneYearAgo.toISOString().split('T')[0];
	
    // --- Adres api ---
	const apiUrl = 'http://localhost:5062/api';


    // --- Funkcja do pobierania listy spółek ---
    async function fetchStocks() {
        const apiStocksUrl = apiUrl + '/stocks';

        try {
            const response = await fetch(apiStocksUrl);
            if (!response.ok) {
                throw new Error(`Błąd HTTP: ${response.status}`);
            }
            const stocks = await response.json();

            // Wyczyść opcję "Ładowanie..."
            stockSelect.innerHTML = '<option value="">-- Wybierz instrument --</option>';

            // Dodaj spółki do dropdownu
            stocks.forEach(stock => {
                const option = document.createElement('option');
                option.value = stock.id; // Używamy ID jako wartości
                option.textContent = `${stock.symbol} (${stock.stockName})`; // Wyświetlamy Symbol i Nazwę
                stockSelect.appendChild(option);
            });

        } catch (error) {
            console.error('Nie udało się pobrać listy spółek:', error);
            stockSelect.innerHTML = '<option value="">Błąd ładowania</option>';
            errorDisplay.textContent = `Nie udało się pobrać listy spółek: ${error.message}`;
        }
    }

    // --- Funkcja do wyświetlania/ukrywania wskaźnika ładowania ---
    function showLoading(isLoading) {
        loadingIndicator.style.display = isLoading ? 'block' : 'none';
    }

    // --- Funkcja do czyszczenia błędów ---
    function clearError() {
        errorDisplay.textContent = '';
    }

    // --- Funkcja do wyświetlania błędów ---
     function showError(message) {
        console.error(message);
        errorDisplay.textContent = message;
        plotlyChartDiv.innerHTML = ''; // Wyczyść poprzedni wykres w razie błędu
        chartOptionsDiv.style.display = 'none'; // Ukryj opcje wykresu
    }


    // --- Nasłuchiwanie na kliknięcie przycisku ---
    showChartButton.addEventListener('click', async () => {
        clearError(); // Wyczyść poprzednie błędy
        const selectedStockId = stockSelect.value;
        const startDate = startDateInput.value;
        const endDate = endDateInput.value;
        const timeframe = timeframeSelect.value;

        if (!selectedStockId) {
            showError("Proszę wybrać instrument.");
            return;
        }
        if (!startDate || !endDate) {
            showError("Proszę wybrać zakres dat.");
            return;
        }

        // Tutaj dodamy logikę pobierania danych i rysowania wykresu (Krok 5)
        console.log(`Pobieranie danych dla: ID=${selectedStockId}, Timeframe=${timeframe}, Od=${startDate}, Do=${endDate}`);
        await fetchAndDrawChart(selectedStockId, timeframe, startDate, endDate);
    });

    // --- Funkcja pobierająca dane i rysująca wykres (na razie pusta) ---
    async function fetchAndDrawChart(stockId, timeframe, startDate, endDate) {
        showLoading(true);
        plotlyChartDiv.innerHTML = ''; // Wyczyść stary wykres
        chartOptionsDiv.style.display = 'none'; // Ukryj opcje na czas ładowania

        const apiStockDataUrl = apiUrl + `/stockdata/${stockId}?timeframe=${timeframe}&startDate=${startDate}&endDate=${endDate}`;
        try {
            const response = await fetch(apiStockDataUrl);
            if (!response.ok) {
                 const errorData = await response.text(); // Spróbuj odczytać treść błędu
                throw new Error(`Błąd HTTP: ${response.status} - ${errorData}`);
            }
            const data = await response.json();

            if (!data || data.length === 0) {
                 showError("Brak danych dla wybranych kryteriów.");
                 showLoading(false);
                 return;
            }

            // Tutaj wywołamy funkcję rysującą Plotly (Krok 5)
            console.log("Otrzymane dane:", data);
            drawPlotlyChart(data); // Przekazujemy pobrane dane
            chartOptionsDiv.style.display = 'block'; // Pokaż opcje wykresu po załadowaniu

        } catch (error) {
            showError(`Nie udało się pobrać danych wykresu: ${error.message}`);
        } finally {
            showLoading(false); // Ukryj wskaźnik ładowania niezależnie od wyniku
        }
    }

     // --- Funkcja rysująca wykres Plotly (na razie szkielet) ---
     function drawPlotlyChart(apiData) {
         console.log("Rysowanie wykresu Plotly...");

         // Przygotowanie danych dla Plotly
         const dates = apiData.map(item => item.timestamp.substring(0, 10)); // Plotly lubi daty jako stringi lub obiekty Date
         const opens = apiData.map(item => item.open);
         const highs = apiData.map(item => item.high);
         const lows = apiData.map(item => item.low);
         const closes = apiData.map(item => item.close);
         const volumes = apiData.map(item => item.volume);

        // 1. Definicja śladu (trace) dla wykresu świecowego
        const candlestickTrace = {
            x: dates,
            open: opens,
            high: highs,
            low: lows,
            close: closes,
            type: 'candlestick',
            name: stockSelect.options[stockSelect.selectedIndex].text.split('(')[0].trim(), // Nazwa z dropdownu
            yaxis: 'y', // Główna oś Y dla ceny
            increasing: {	 // Kolory świec
				line: {color: 'black', width: 1},
				fillcolor: 'white'
			},
            decreasing: {
				line: {color: 'black', width: 1},
				fillcolor: 'black'
			}
        };

        // 2. Definicja śladu dla wolumenu (słupkowy)
        const volumeTrace = {
            x: dates,
            y: volumes,
            type: 'bar',
            name: 'Wolumen',
            yaxis: 'y2', // Druga oś Y dla wolumenu
            marker: {
                color: 'rgba(100, 100, 255, 0.7)' // Kolor słupków wolumenu
            }
        };

        // 3. Konfiguracja layoutu (wyglądu) wykresu
        const layout = {
            title: `Wykres dla ${stockSelect.options[stockSelect.selectedIndex].text}`,
            xaxis: {
                rangeslider: { visible: false }, // Ukrywamy domyślny suwak zakresu pod osią X
                type: 'category',
				nticks: 10				
            },
            yaxis: {
                title: 'Cena',
                domain: [0.25, 1] // Oś ceny zajmuje górne 75% wysokości (od 25% do 100%)
            },
            yaxis2: {
                title: 'Wolumen',
                domain: [0, 0.2], // Oś wolumenu zajmuje dolne 20% wysokości
                showticklabels: true, // Pokaż etykiety na osi wolumenu
                side: 'right' // Można dać oś wolumenu po prawej
            },
            hovermode: 'x unified', // Kluczowe dla crosshair - pokazuje info dla wszystkich śladów na danej dacie
            dragmode: 'zoom', // Umożliwia zoom przez zaznaczenie
            legend: { // Położenie legendy
                x: 0,
                y: 1.1,
                orientation: "h" // Horyzontalna legenda nad wykresem
            }
            //margin: { l: 50, r: 50, t: 50, b: 50 } // Marginesy
        };

        // 4. Rysowanie wykresu w kontenerze 'plotlyChartDiv'
        Plotly.newPlot(plotlyChartDiv, [candlestickTrace, volumeTrace], layout, {responsive: true}); // responsive: true dostosuje rozmiar

        console.log("Wykres narysowany.");
     }

    // --- Inicjalizacja ---
    fetchStocks(); // Pobierz listę spółek przy starcie strony

}); // Koniec nasłuchiwania na DOMContentLoaded