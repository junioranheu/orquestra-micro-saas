export const MODULES = {
    Scheduling: 1,
    IntegrationWhatsApp: 2,
    CostumerFollowUp: 3,
    Invoice: 4,
    Sales: 5
} as const;

export type Module = typeof MODULES[keyof typeof MODULES];