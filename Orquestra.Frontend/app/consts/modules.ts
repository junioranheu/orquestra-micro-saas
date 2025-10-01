export const MODULES = {
    Scheduling: 1,
    Sales: 2
} as const;

export type Module = typeof MODULES[keyof typeof MODULES];