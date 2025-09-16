import { iModalCustomPosition } from '@/app/components/modal/generic';
import { MouseEvent } from 'react';

export default function handleGetModalClickPosition(event: MouseEvent<HTMLDivElement | HTMLSpanElement>, topOffset: number = 125): iModalCustomPosition {
    // console.log('event', event);
    const { clientX, clientY } = event;
    const modalClickPosition = { top: clientY + topOffset, left: clientX } as iModalCustomPosition;

    return modalClickPosition;
}