'use client';
import { iTourGroup } from '@/app/hooks/useGetMenuGroups';
import { TourProvider } from '@reactour/tour';
import { ReactNode } from 'react';

interface Props {
    steps: iTourGroup[];
    children: ReactNode;
}

export function CustomTourProvider({ steps, children }: Props) {
    return (
        <TourProvider
            key={steps.length}
            steps={steps}
            styles={{
                popover: (base) => ({
                    ...base,
                    backgroundColor: 'var(--white)',
                    border: '1px solid var(--gray-light)',
                    color: 'var(--black)',
                    borderRadius: 'var(--border-radius)',
                    padding: '1rem 2.5rem',
                    fontFamily: 'inherit'
                }),
                maskArea: (base) => ({
                    ...base,
                    stroke: 'rgba(255, 255, 255, 0.2)'
                }),
                maskWrapper: (base) => ({
                    ...base,
                    color: 'rgba(0, 0, 0, 0.85)'
                }),
                badge: (base) => ({
                    ...base,
                    backgroundColor: 'var(--contrast)'
                }),
                close: (base) => ({
                    ...base,
                    color: 'var(--contrast)'
                }),
                arrow: (base) => ({
                    ...base,
                    color: 'var(--main)',
                    opacity: 0.7
                }),
                dot: (base, state) => ({
                    ...base,
                    backgroundColor: state?.current ? 'var(--main)' : 'var(--gray)',
                    opacity: state?.current ? 1 : 0.35,
                })
            }}
            showBadge={false}
        >
            {children}
        </TourProvider>
    )
}