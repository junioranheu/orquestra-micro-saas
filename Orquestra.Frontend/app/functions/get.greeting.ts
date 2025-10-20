import { DATE_STYLE, handleFormatDate } from './format.date';
import { handleGetDateBrazil } from './get.date.brazil';
import { handleGenerateEvenOrOdd } from './get.evenOrOdd';

interface iProps {
    mustIncludeUmUma: boolean;
}

export function handleGetDayWeekGreeting({ mustIncludeUmUma }: iProps): string {
    const date = handleGetDateBrazil();
    const dayOfWeek = date.getDay(); // 0 = domingo, 1 = segunda, ..., 6 = sábado;
    const dateStr = handleFormatDate(date, DATE_STYLE.DIA_DA_SEMANA);

    if (mustIncludeUmUma) {
        const greeting = dayOfWeek >= 1 && dayOfWeek <= 5 ? 'Uma boa' : 'Um bom';
        const output = `${greeting} ${dateStr}`;

        return output;
    }

    const greeting = dayOfWeek >= 1 && dayOfWeek <= 5 ? 'Boa' : 'Bom';
    const output = `${greeting} ${dateStr}`;

    return output;
}

export function handleGetTimeGreeting({ mustIncludeUmUma }: iProps): string {
    const date = handleGetDateBrazil();
    const hour = date.getHours();

    let greeting = '';

    if (hour >= 5 && hour < 12) {
        greeting = 'Bom dia';
    }
    else if (hour >= 12 && hour < 18) {
        greeting = 'Boa tarde';
    }
    else {
        greeting = 'Boa noite';
    }

    if (mustIncludeUmUma) {
        const article = greeting.startsWith('Boa') ? 'Uma' : 'Um';
        return `${article} ${greeting.toLowerCase()}`;
    }

    return greeting;
}

export function handleGetRandomGreeting({ mustIncludeUmUma }: iProps): string {
    return handleGenerateEvenOrOdd() ? handleGetDayWeekGreeting({ mustIncludeUmUma }) : handleGetTimeGreeting({ mustIncludeUmUma });
}