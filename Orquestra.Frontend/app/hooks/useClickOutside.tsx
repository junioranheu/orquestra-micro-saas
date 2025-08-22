import { Dispatch, RefObject, SetStateAction, useCallback, useEffect } from 'react';

export default function useClickOutside(ref: RefObject<HTMLElement>, setter: Dispatch<SetStateAction<boolean>>): void {

    const handleClickOutside = useCallback((event: MouseEvent) => {
        if (ref.current && !ref.current.contains(event.target as Node)) {
            setter(false);
        }
    }, [ref, setter]);

    useEffect(() => {
        document.addEventListener('click', handleClickOutside, true);

        return () => {
            document.removeEventListener('click', handleClickOutside, true);
        };
    }, [handleClickOutside]);

}