// js/apiService.js
import { config } from './config.js';

const apiService = {
    async fetchStocks() {
        const url = `${config.apiUrlBase}/stocks`;
        console.log(`Fetching stocks from: ${url}`);
        try {
            const response = await fetch(url);
            if (!response.ok) {
                throw new Error(`Błąd HTTP: ${response.status} przy pobieraniu listy spółek`);
            }
            return await response.json();
        } catch (error) {
            console.error(`Fetch error for ${url}:`, error);
            throw error;
        }
    },

    async fetchStockData(stockId, timeframe, startDate, endDate) {
        const url = `${config.apiUrlBase}/stockdata/${stockId}?timeframe=${timeframe}&startDate=${startDate}&endDate=${endDate}`;
        console.log(`Fetching stock data from: ${url}`);
         try {
            const response = await fetch(url);
            if (!response.ok) {
                const errorData = await response.text();
                throw new Error(`Błąd HTTP: ${response.status} - ${errorData}`);
            }
            return await response.json();
        } catch (error) {
            console.error(`Fetch error for ${url}:`, error);
            throw error;
        }
    }
};

export { apiService };