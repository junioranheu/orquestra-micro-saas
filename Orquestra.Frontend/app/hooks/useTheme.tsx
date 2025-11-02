import styles from '@/app/(routes)/(auth)/usuario/configuracoes/tabs/personalizacao/index.module.scss';
import SYSTEM from '@/app/consts/system';
import { useEffect } from 'react';

export const THEMES = [
    {
        id: 'padrao',
        label: 'Padrão',
        className: styles.themeDefault,
        isUsable: true
    },
    {
        id: 'escuro',
        label: 'Escuro',
        className: styles.themeDark,
        isUsable: false,
        vars: {
            '--main-light': '#15171A',
            '--contrast': '#6c63ff',
            '--contrast-light': '#2a2a45',
            '--black': '#f7f9fc',
            '--white': '#15171A',
            '--white-og': '#15171A',
            '--muted': '#c1c1c1',
            '--gray': '#2a2a2a',
            '--gray-light': '#3a3a3a',
            '--gray-dark': '#d0d0d0',
            '--cream': '#1a1a1a',
            '--gradient': 'linear-gradient(120deg, #1b1b1b, #202020, #1a1a1a, #2a2a2a)',
            '--box-shadow': '0 4px 8px rgba(255,255,255,0.05), 0 6px 20px rgba(255,255,255,0.05)',
            '--box-shadow-dark': '0 10px 40px rgba(0,0,0,0.5), 0 4px 6px rgba(0,0,0,0.2)',
            '--filter-main': 'invert(80%) sepia(30%) saturate(500%) hue-rotate(100deg) brightness(120%) contrast(100%)',
        }
    }
];

export default function useTheme(): void {

    useEffect(() => {
        const saved = localStorage.getItem(SYSTEM.LOCAL_STORAGE_THEME) || 'padrao';
        handleApplyTheme(saved);
    }, []);

}

export function handleApplyTheme(themeId: string) {
    const root = document.documentElement;
    const dark = THEMES.find((t) => t.id === 'escuro')!;
    const selected = THEMES.find((t) => t.id === themeId);

    if (themeId === 'escuro' && selected?.vars) {
        Object.entries(selected.vars).forEach(([key, value]) => {
            root.style.setProperty(key, value);
        });
    } else {
        // Resetando todas as vars do modo escuro (voltando pro padrão);
        Object.keys(dark.vars!).forEach((key) => {
            root.style.removeProperty(key);
        });
    }
}