import { iDropdownOption } from '@/app/components/input/drop-down';

export const COLORS = [
    { value: '#FF6B6B', label: 'Coral suave' },
    { value: '#4ECDC4', label: 'Turquesa' },
    { value: '#6C5CE7', label: 'Violeta' },
    { value: '#FFD93D', label: 'Amarelo dourado' },
    { value: '#FF9F1C', label: 'Laranja queimado' },
    { value: '#38B000', label: 'Verde folha' },
    { value: '#00A8E8', label: 'Azul celeste' },
    { value: '#9D4EDD', label: 'Roxo real' },
    { value: '#F15BB5', label: 'Rosa chiclete' },
    { value: '#FFB5A7', label: 'Pêssego' },
    { value: '#B5E48C', label: 'Verde pastel' },
    { value: '#CAF0F8', label: 'Azul pastel' },
].sort((a, b) => a.label.localeCompare(b.label)) as iDropdownOption<string>[];

export type Color = typeof COLORS[keyof typeof COLORS];