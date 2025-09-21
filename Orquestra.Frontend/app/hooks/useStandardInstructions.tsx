import SYSTEM from '@/app/consts/system';
import { useEffect } from 'react';

export default function useStandardIntructions() {

    useEffect(() => {
        function handleScrollTop() {
            document.body.scrollTop = document.documentElement.scrollTop = 0;
        }

        function handleConsole() {
            setTimeout(function () {
                if (process.env.NODE_ENV === 'production') {
                    console.clear();
                }

                console.log(`%c${SYSTEM.NAME}: ${SYSTEM.DESCRIPTION}`, 'color: #0b996f; font-family:monospace; font-size: 26px');
            }, 2000);
        }

        function handleBlockConsoleInspectTool() {
            if (process.env.NODE_ENV === 'production') {
                document.addEventListener('contextmenu', function (e) {
                    e.preventDefault();
                });

                document.onkeydown = function (e) {
                    if (e.keyCode == 123) {
                        return false;
                    }

                    if (e.ctrlKey && e.shiftKey && e.keyCode == 'I'.charCodeAt(0)) {
                        return false;
                    }

                    if (e.ctrlKey && e.shiftKey && e.keyCode == 'C'.charCodeAt(0)) {
                        return false;
                    }

                    if (e.ctrlKey && e.shiftKey && e.keyCode == 'J'.charCodeAt(0)) {
                        return false;
                    }

                    if (e.ctrlKey && e.keyCode == 'U'.charCodeAt(0)) {
                        return false;
                    }
                }
            }
        }

        handleScrollTop();
        handleConsole();
        handleBlockConsoleInspectTool();
    }, []);

}