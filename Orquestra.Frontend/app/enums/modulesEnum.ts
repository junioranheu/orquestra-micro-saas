export enum MODULE_ENUM {
    Member = 1,
    Client = 2,
    Scheduling = 3,
    IntegrationWhatsApp = 4,
    CostumerFollowUp = 5,
    Invoice = 6,
    Sales = 7,
    Quote = 8,
    ServiceOrder = 9,
    Inventory = 10
}

export type Module = typeof MODULE_ENUM[keyof typeof MODULE_ENUM];