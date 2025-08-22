import { useEffect } from 'react';

type KeyType =
    | 'Escape'
    | 'Enter'
    | 'ArrowUp'
    | 'ArrowDown'
    | 'ArrowLeft'
    | 'ArrowRight'
    | 'Backspace'
    | 'Tab'
    | 'Delete'
    | 'Home'
    | 'End'
    | 'PageUp'
    | 'PageDown'
    | 'Control'
    | 'Alt'
    | 'Shift';

export default function useKeyPress(targetKey: KeyType, onKeyPress: () => void): void {

    useEffect(() => {
        function handleKeyDown(event: KeyboardEvent): void {
            // console.log(event.key, targetKey, event.key === targetKey);

            if (event.key === targetKey) {
                onKeyPress();
            }
        }

        document.addEventListener('keydown', handleKeyDown);

        return () => {
            document.removeEventListener('keydown', handleKeyDown);
        };
    }, [targetKey, onKeyPress]);

}