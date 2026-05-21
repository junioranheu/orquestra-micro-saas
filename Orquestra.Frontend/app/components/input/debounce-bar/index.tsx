'use client';
import { useEffect, useRef, useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    value: string;
    debounceSeconds: number;
    onDebounce: (value: string) => void;
}

/**
 * Componente reutilizável de debounce com barra visual de countdown.
 * Monitora `value`; após `debounceSeconds` sem alteração, dispara `onDebounce`.
 */
export default function DebounceBar({ value, debounceSeconds, onDebounce }: iProps) {

    const debounceTimerRef = useRef<ReturnType<typeof setTimeout> | null>(null);
    const countdownIntervalRef = useRef<ReturnType<typeof setInterval> | null>(null);
    const [debounceCountdown, setDebounceCountdown] = useState<number | null>(null);

    useEffect(() => {
        if (debounceTimerRef.current) {
            clearTimeout(debounceTimerRef.current);
        }

        if (countdownIntervalRef.current) {
            clearInterval(countdownIntervalRef.current);
        }

        const trimmed = value?.trim() ?? '';

        if (trimmed.length > 0) {
            setDebounceCountdown(debounceSeconds);

            countdownIntervalRef.current = setInterval(() => {
                setDebounceCountdown(prev => {
                    if (prev === null || prev <= 0.1) {
                        return null;
                    }

                    return Math.round((prev - 0.1) * 10) / 10;
                });
            }, 100);

            debounceTimerRef.current = setTimeout(() => {
                if (countdownIntervalRef.current) {
                    clearInterval(countdownIntervalRef.current);
                }

                setDebounceCountdown(null);
                onDebounce(trimmed);
            }, debounceSeconds * 1000);
        } else {
            setDebounceCountdown(null);
        }

        return () => {
            if (debounceTimerRef.current) {
                clearTimeout(debounceTimerRef.current);
            }

            if (countdownIntervalRef.current) {
                clearInterval(countdownIntervalRef.current);
            }
        };
    }, [value, debounceSeconds, onDebounce]);

    if (debounceCountdown === null) {
        return null;
    }

    return (
        <div className={styles.debounceBar}>
            <div
                className={styles.debounceBarFill}
                style={{ width: `${(debounceCountdown / debounceSeconds) * 100}%` }}
            />

            <span className={styles.debounceBarLabel}>
                Buscando em {Math.ceil(debounceCountdown)}s…
            </span>
        </div>
    )
}