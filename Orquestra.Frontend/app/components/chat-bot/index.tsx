'use client';
import { iMe } from '@/app/api/consts/auth';
import Icon from '@/app/components/icon';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { handleGetFirstName } from '@/app/functions/get.formatUserName';
import { handleGetTimeGreeting } from '@/app/functions/get.greeting';
import { useIsOpenChatbot } from '@/app/hooks/contexts/useGlobalContext';
import Tippy from '@tippyjs/react';
import { FormEvent, useEffect, useRef, useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    me: iMe | undefined;
}

interface iMessage {
    role: 'user' | 'bot';
    text: string
}

export default function ChatBot({ me }: iProps) {

    const API_KEY = process.env.NEXT_PUBLIC_GOOGLE_AI_API_KEY;

    const [isOpenChatbot, setIsOpenChatbot] = useIsOpenChatbot();
    const [messages, setMessages] = useState<iMessage[]>([]);
    const [input, setInput] = useState<string>('');
    const [loading, setLoading] = useState<boolean>(false);
    const chatEndRef = useRef<HTMLDivElement | null>(null);

    useEffect(() => {
        if (!me?.userName) {
            return;
        }

        setMessages((prev) => [
            ...prev,
            { role: 'bot', text: `Olá, ${handleGetFirstName(me?.userName)}! ${handleGetTimeGreeting({ mustIncludeUmUma: false })}.<br/>Eu sou o <b>${SYSTEM.MASCOT}</b>, seu assistente virtual. 🐱<br/>Como posso te ajudar hoje?` },
        ]);
    }, [me]);

    useEffect(() => {
        chatEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    }, [messages]);

    async function handleSendMessage(e?: FormEvent) {
        if (e) {
            e.preventDefault();
        }

        if (!input.trim()) {
            return;
        }

        if (!API_KEY) {
            setMessages((prev) => [
                ...prev,
                { role: 'bot', text: 'Nenhuma API key encontrada' },
            ]);

            return;
        }

        // Checar plano atual;
        if (me?.currentMainCompany?.planType?.toString() === '1') {
            setMessages((prev) => [
                ...prev,
                { role: 'bot', text: `Opa, parece que seu plano atual é o <b>básico</b>. <a href="${ROUTES.EMPRESA_USO_E_PLANO}">Faça um upgrade no seu plano</a> para usar o assistente virtual!` },
            ]);

            return;
        }

        const userMessage = input.trim();
        setMessages((prev) => [...prev, { role: 'user', text: userMessage }]);
        setInput('');
        setLoading(true);

        if (!handleCheckIfIsAboutThePlataform(userMessage)) {
            setMessages(prev => [...prev, { role: 'bot', text: `Desculpe — eu só respondo sobre a plataforma ${SYSTEM.NAME}.` }]);
            setLoading(false);
            return;
        }

        try {
            const url = `https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent`;
            const SYSTEM_DEFAULT_PROMPT = `Você é o Maestro, o assistente virtual oficial do sistema ${SYSTEM.NAME} — uma plataforma que ajuda empresas a gerenciar agendamentos, serviços e clientes de forma simples e eficiente. Você é prestativo, responde de forma objetiva e mantém um tom natural e leve nas conversas.`;

            const res = await fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'x-goog-api-key': API_KEY
                },
                body: JSON.stringify({
                    contents: [
                        {
                            role: 'user',
                            parts: [
                                { text: SYSTEM_DEFAULT_PROMPT },
                                { text: userMessage }
                            ]
                        }
                    ]
                })
            });

            if (!res.ok) {
                const text = await res.text();
                console.error('gemini err', res.status, text);
                setMessages((prev) => [...prev, { role: 'bot', text: `Erro da API: ${res.status}` }]);
                return;
            }

            const data = await res.json();
            const reply = data?.candidates?.[0]?.content?.parts?.[0]?.text ?? 'Não entendi 🤔';
            const replyNormalized = reply.replace(/\*\*(.*?)\*\*/g, '<b>$1</b>');
            setMessages((prev) => [...prev, { role: 'bot', text: replyNormalized }]);
        } catch (err) {
            console.error(err);
            setMessages((prev) => [...prev, { role: 'bot', text: 'Erro ao conectar com o bot 😞' }]);
        } finally {
            setLoading(false);
        }
    }

    function handleCheckIfIsAboutThePlataform(text: string) {
        const s = text.toLowerCase();

        const keywords = [
            'oi', 'olá', 'tudo bem', 'como vai', 'beleza',
            'agend', 'serviço', 'servico', 'cliente', 'horár', 'horar', 'cancel', 'marc',
            'remarc', 'agenda', 'notifica', 'notificação', 'notificacao', 'pagamento',
            'fatura', 'plano', 'empresa', 'cadast', 'login', 'senha', 'checkout',
            'se chama', 'seu nome', 'hora', 'evento', 'configura', 'cliente', 'follow',
            'ordem', 'estoque', 'nota', 'ajuda', SYSTEM.NAME, 'obrigad', 'whatsapp', 'zap',
            'consult', 'paciente', 'colaborador', 'equipe'
        ];

        // Se achar qualquer keyword retorna true;
        for (const kw of keywords) {
            if (s.includes(kw)) {
                return true;
            }
        }

        return false;
    }

    return (
        <div className={styles.chatbotWrapper}>
            {
                isOpenChatbot ? (
                    <div className={styles.window}>
                        <div className={styles.header}>
                            <div className={styles.headerTitle}>{SYSTEM.MASCOT}, seu assistente virtual</div>

                            <Tippy content='Fechar'>
                                <button className={styles.closeBtn} onClick={() => setIsOpenChatbot(false)}>×</button>
                            </Tippy>
                        </div>

                        <div className={styles.body}>
                            {
                                messages.map((msg, i) => (
                                    <div key={i} className={`${styles.messageRow} ${msg.role === 'user' ? 'user' : 'bot'}`}>
                                        <div
                                            className={`${styles.bubble} ${msg.role === 'user' ? styles.user : styles.bot}`}
                                            dangerouslySetInnerHTML={{ __html: msg.text }}
                                        />
                                    </div>
                                ))
                            }

                            <div ref={chatEndRef} />
                        </div>

                        <form onSubmit={handleSendMessage} className={styles.footer}>
                            <input
                                className={styles.input}
                                type='text'
                                value={input}
                                onChange={(e) => setInput(e.target.value)}
                                placeholder='Digite uma mensagem...'
                            />

                            <button className={styles.sendBtn} type='submit' disabled={loading}>
                                {
                                    loading ? (
                                        <Tippy content='Estou pensando... pera aí'>
                                            <div className={styles.loading}>
                                                <div className={styles.dot}></div>
                                                <div className={styles.dot}></div>
                                                <div className={styles.dot}></div>
                                            </div>
                                        </Tippy>
                                    ) : (
                                        <span>Enviar</span>
                                    )
                                }
                            </button>
                        </form>
                    </div>
                ) : (
                    <Tippy content={`Converse com o ${SYSTEM.MASCOT} e tire todas suas dúvidas sobre o ${SYSTEM.NAME}!`} placement='left'>
                        <button className={styles.bubbleButton} onClick={() => setIsOpenChatbot(true)}>
                            <Icon icon='message-square' />
                        </button>
                    </Tippy>
                )
            }
        </div>
    )
}