import handleGetDateBrazil from './get.date.brazil';

export function handleIsBeforeToday(date: Date): boolean {
    const today = new Date();
    const dateToCompare = new Date(date.getFullYear(), date.getMonth(), date.getDate());
    const todayWithoutTime = new Date(today.getFullYear(), today.getMonth(), today.getDate());

    return dateToCompare.getTime() < todayWithoutTime.getTime();
}

export function handleIsBeforeTodayWithTime(date: Date): boolean {
    return date < handleGetDateBrazil();
}