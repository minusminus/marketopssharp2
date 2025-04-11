// js/indicators/channelIndicator.js
import { getPriceChannelTraces } from '../chartStyleConfig.js';

const channelIndicator = {
    /**
     * Oblicza Kanał Cenowy (ręcznie).
     * @param {object} inputData Obiekt z tablicami { high, low, ... }
     * @param {number} period Okres kanału.
     * @returns {{upper: Array<number|null>, lower: Array<number|null>}|null} Obiekt z tablicami lub null.
     */
    calculate(inputData, period) {
        if (!inputData || !inputData.high || !inputData.low || inputData.high.length < period) {
            return null;
        }
        const upper = [];
        const lower = [];
        const high = inputData.high;
        const low = inputData.low;
        const len = high.length;

        for (let i = 0; i < len; i++) {
            if (i < period - 1) {
                upper.push(null);
                lower.push(null);
            } else {
                const highsSlice = high.slice(i - period + 1, i + 1);
                const lowsSlice = low.slice(i - period + 1, i + 1);
                upper.push(Math.max(...highsSlice));
                lower.push(Math.min(...lowsSlice));
            }
        }
        return { upper, lower };
    },

    /**
     * Generuje ślady Plotly dla Kanału Cenowego.
     * @param {Array<string>} allDates Pełna tablica dat.
     * @param {{upper: Array<number|null>, lower: Array<number|null>}} indicatorData Wynik z metody calculate.
     * @param {object} state Stan wskaźnika { period, ... }.
     * @returns {Array<object>} Tablica obiektów śladów Plotly.
     */
    getTraces(allDates, indicatorData, state) {
        if (indicatorData && indicatorData.upper.length === allDates.length) {
             // Nasza kalkulacja już ma nulle, nie potrzebujemy paddingu
            return getPriceChannelTraces(allDates, indicatorData.upper, indicatorData.lower, state.period);
        } else {
             console.warn(`Channel getTraces: Niezgodna długość danych (${indicatorData?.upper?.length} vs ${allDates.length})`);
            return [];
        }
    }
};

export { channelIndicator };