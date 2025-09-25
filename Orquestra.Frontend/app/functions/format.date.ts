import { handleGetDateBrazil } from './get.date.brazil';

export const DATE_STYLE = {
    DIA_MES_ANO: 'DIA_MES_ANO',
    DETALHADO: 'DETALHADO',
    MES_EXTENSO_E_ANO: 'MES_EXTENSO_E_ANO',
    DIA_DA_SEMANA_E_DIA_DO_MES: 'DIA_DA_SEMANA_E_DIA_DO_MES',
    DIA_DA_SEMANA: 'DIA_DA_SEMANA',
    DIA_DA_SEMANA_E_DIA_DO_MES_E_ANO: 'DIA_DA_SEMANA_E_DIA_DO_MES_E_ANO',
    DIA_MES_ANO_HORA_MINUTO_SEGUNDO: 'DIA_MES_ANO_HORA_MINUTO_SEGUNDO',
    HORA_MINUTO: 'HORA_MINUTO'
} as const;

export function handleFormatDate(date: Date | string | undefined, style: keyof typeof DATE_STYLE): string {
    try {
        if (!date) {
            return '-';
        }

        const locale = 'pt-BR';
        const isBr = true;
        const today = 'Hoje';
        const yesterday = 'Ontem';
        const tomorrow = 'Amanhã';

        let dateFormatted = '';
        const dataObj = typeof date === 'string' ? new Date(date) : date;

        switch (style) {
            case DATE_STYLE.DIA_MES_ANO:
                dateFormatted = dataObj.toLocaleDateString(locale, { day: 'numeric', month: 'numeric', year: 'numeric' });
                break;

            case DATE_STYLE.DETALHADO:
                const diffDays = handleCheckDiffInDays(new Date(), dataObj);
                // console.log(diffDays);

                if (diffDays === 0) {
                    dateFormatted = `${today}, ${dataObj.toLocaleTimeString(locale, { hour: 'numeric', minute: 'numeric', second: 'numeric', hour12: !isBr })}`;
                } else if (diffDays === 1) {
                    dateFormatted = `${yesterday}, ${dataObj.toLocaleTimeString(locale, { hour: 'numeric', minute: 'numeric', second: 'numeric', hour12: !isBr })}`;
                } else if (diffDays === -1) {
                    dateFormatted = `${tomorrow}, ${dataObj.toLocaleTimeString(locale, { hour: 'numeric', minute: 'numeric', second: 'numeric', hour12: !isBr })}`;
                } else {
                    dateFormatted = dataObj.toLocaleTimeString(locale, { day: 'numeric', month: 'numeric', year: 'numeric', hour: 'numeric', minute: 'numeric', second: 'numeric', hour12: !isBr });
                }
                break;

            case DATE_STYLE.MES_EXTENSO_E_ANO:
                dateFormatted = dataObj.toLocaleDateString(locale, { month: 'long', year: 'numeric' });
                break;

            case DATE_STYLE.DIA_DA_SEMANA_E_DIA_DO_MES:
                dateFormatted = dataObj.toLocaleDateString(locale, { weekday: 'long', day: 'numeric', month: 'long' });
                break;

            case DATE_STYLE.DIA_DA_SEMANA:
                dateFormatted = dataObj.toLocaleDateString(locale, { weekday: 'long' });
                break;

            case DATE_STYLE.DIA_DA_SEMANA_E_DIA_DO_MES_E_ANO:
                dateFormatted = dataObj.toLocaleDateString(locale, { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' });
                break;

            case DATE_STYLE.DIA_MES_ANO_HORA_MINUTO_SEGUNDO:
                dateFormatted = dataObj.toLocaleDateString(locale, { day: 'numeric', month: 'numeric', year: 'numeric', hour: 'numeric', minute: 'numeric', second: 'numeric', hour12: false });
                break;

            case DATE_STYLE.HORA_MINUTO:
                dateFormatted = dataObj.toLocaleTimeString(locale, { hour: '2-digit', minute: '2-digit', hour12: false });
                break;

            default:
                dateFormatted = '-';
        }

        return dateFormatted;
    } catch {
        return '-';
    }
}

export function handleCheckDiffInDays(data1: Date, data2: Date): number {
    const oneDayMs = 1000 * 60 * 60 * 24;

    const d1 = new Date(data1.getFullYear(), data1.getMonth(), data1.getDate());
    const d2 = new Date(data2.getFullYear(), data2.getMonth(), data2.getDate());

    return Math.floor((d1.getTime() - d2.getTime()) / oneDayMs);
}

export function handleIsBeforeToday(date: Date): boolean {
    const today = new Date();
    const dateToCompare = new Date(date.getFullYear(), date.getMonth(), date.getDate());
    const todayWithoutTime = new Date(today.getFullYear(), today.getMonth(), today.getDate());

    return dateToCompare.getTime() < todayWithoutTime.getTime();
}

export function handleIsBeforeTodayWithTime(date: Date): boolean {
    return date < handleGetDateBrazil();
}

/**
 * Formata um objeto Date em uma string no formato "YYYY-MM-DD",
 * compatível com campos HTML do tipo date (`type="date"`).
 *
 * @param date - O objeto Date que será formatado.
 * @returns Uma string representando a data no formato "YYYY-MM-DD".
 */
export function handleFormatDateToInputValue(date: Date): string {
    return new Date(date).toISOString().split("T")[0];
}

/**
 * Formata um objeto Date em uma string no formato "YYYY-MM-DDTHH:mm",
 * compatível com campos HTML do tipo datetime-local (`type="datetime-local"`).
 *
 * @param date - O objeto Date que será formatado.
 * @returns Uma string representando a data e hora no formato "YYYY-MM-DDTHH:mm".
 */
export function handleFormatDateTimeToInputValue(date: Date): string {
    const d = new Date(date);
    const pad = (n: number) => n.toString().padStart(2, "0");

    const year = d.getFullYear();
    const month = pad(d.getMonth() + 1);
    const day = pad(d.getDate());
    const hours = pad(d.getHours());
    const minutes = pad(d.getMinutes());

    return `${year}-${month}-${day}T${hours}:${minutes}`;
}