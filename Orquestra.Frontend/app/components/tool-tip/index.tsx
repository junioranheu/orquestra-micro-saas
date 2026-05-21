'use client';
import * as Tooltip from '@radix-ui/react-tooltip';
import { ReactNode } from 'react';

type Placement = | 'top' | 'bottom' | 'left' | 'right';

interface iProps {
    children: ReactNode;
    content: ReactNode;
    placement?: Placement;
    interactive?: boolean;
};

export default function Tippy({
    children,
    content,
    placement = 'top',
    interactive = false
}: iProps) {
    return (
        <Tooltip.Provider delayDuration={150}>
            <Tooltip.Root>
                <Tooltip.Trigger asChild>
                    {children}
                </Tooltip.Trigger>

                <Tooltip.Portal>
                    <Tooltip.Content
                        side={placement}
                        sideOffset={6}
                        className='tooltip'
                        onPointerDownOutside={(e) => {
                            if (interactive) {
                                e.preventDefault();
                            }
                        }}
                    >
                        {content}

                        <Tooltip.Arrow />
                    </Tooltip.Content>
                </Tooltip.Portal>
            </Tooltip.Root>
        </Tooltip.Provider>
    )
}