export const MODULES = {
    Members: 1,
    Clients: 2,
    Scheduling: 3,
    IntegrationWhatsApp: 4,
    CostumerFollowUp: 5,
    Invoice: 6,
    Sales: 7,
    Quote: 8,
    ServiceOrder: 9,
    Inventory: 10
} as const;

export type Module = typeof MODULES[keyof typeof MODULES];