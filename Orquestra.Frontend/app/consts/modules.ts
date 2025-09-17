export const MODULES = {
    Scheduling: 'Scheduling',
    Sales: 'Sales',
} as const;

export type Module = typeof MODULES[keyof typeof MODULES];