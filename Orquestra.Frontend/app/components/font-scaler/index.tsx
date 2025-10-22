'use client';
import SYSTEM from '@/app/consts/system';
import { useEffect, useState } from 'react';
import styles from './index.module.scss';

export default function FontScaler() {

    const [scale, setScale] = useState<number>(1);

    // Carrega valor salvo;
    useEffect(() => {
        const saved = localStorage.getItem(SYSTEM.LOCAL_STORAGE_FONT_SIZE);

        if (saved) {
            const value = parseFloat(saved);

            if (!isNaN(value)) {
                setScale(value);
            }
        }
    }, []);

    // Aplica zoom global + salva;
    useEffect(() => {
        document.body.style.zoom = String(scale);
        document.body.style.fontSize = `${scale * 100}%`;
        localStorage.setItem(SYSTEM.LOCAL_STORAGE_FONT_SIZE, String(scale));
    }, [scale]);

    function handleIncrease() {
        setScale(prev => Math.min(1.5, +(prev + 0.1).toFixed(1)));
    }

    function handleDecrease() {
        setScale(prev => Math.max(0.8, +(prev - 0.1).toFixed(1)));
    }

    function handleReset() {
        setScale(1);
    }

    return (
        <div className={styles.fontScaler}>
            <button onClick={handleDecrease}>A-</button>
            <button onClick={handleIncrease}>A+</button>
            <button onClick={handleReset}>Resetar</button>
            <span>{(scale * 100).toFixed(0)}%</span>
        </div>
    )
}