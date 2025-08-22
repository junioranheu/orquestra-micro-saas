export default interface iError {
    requestPath: {
        value: string;
        hasValue: boolean;
    };

    code: number;
    date: Date | string;
    messages: string[] | null;
    error: boolean;
}