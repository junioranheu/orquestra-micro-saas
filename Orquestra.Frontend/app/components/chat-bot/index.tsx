'use client';
import { HELP_TOPICS } from '@/app/(routes)/(public)/(etc)/ajuda/page';
import { iMe } from '@/app/api/consts/auth';
import ImgMaestro from '@/app/assets/png/maestro.png';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { handleGetFirstName } from '@/app/functions/get.formatUserName';
import { handleGetTimeGreeting } from '@/app/functions/get.greeting';
import { useIsOpenChatbot } from '@/app/hooks/contexts/useGlobalContext';
import Tippy from '@tippyjs/react';
import Image from 'next/image';
import { FormEvent, useEffect, useRef, useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    me: iMe | undefined;
    showButtonAbsolute: boolean;
}

interface iMessage {
    role: 'user' | 'bot';
    text: string
}

export const TIPPY_CHATBOT = `Converse com o ${SYSTEM.MASCOT} e tire todas suas dúvidas sobre o ${SYSTEM.NAME}!`;

export default function ChatBot({ me, showButtonAbsolute }: iProps) {

    const API_KEY = process.env.NEXT_PUBLIC_GOOGLE_AI_API_KEY;

    const [isOpenChatbot, setIsOpenChatbot] = useIsOpenChatbot();
    const [messages, setMessages] = useState<iMessage[]>([]);
    const [input, setInput] = useState<string>('');
    const [loading, setLoading] = useState<boolean>(false);
    const chatEndRef = useRef<HTMLDivElement | null>(null);
    const hasGreetedRef = useRef(false);
    const conversationRef = useRef<any[]>([]);

    useEffect(() => {
        if (hasGreetedRef.current || !me?.userName) {
            return;
        }

        setMessages((prev) => [
            ...prev,
            {
                role: 'bot',
                text: `Olá, ${handleGetFirstName(me?.userName)}! ${handleGetTimeGreeting({ mustIncludeUmUma: false })}.<br/>Eu sou o <b>${SYSTEM.MASCOT}</b>, seu assistente virtual. 🐱<br/>Como posso te ajudar hoje?`
            },
        ]);

        hasGreetedRef.current = true;
    }, [me]);

    useEffect(() => {
        chatEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    }, [messages]);

    async function handleSendMessage(e?: FormEvent) {
        e?.preventDefault();

        if (!input.trim()) {
            return;
        }

        if (!API_KEY) {
            setMessages(p => [...p, { role: 'bot', text: 'Nenhuma API key encontrada 💀' }]);
            return setLoading(false);
        }

        const userMessage = input.trim();
        setMessages(p => [...p, { role: 'user', text: userMessage }]);
        setInput('');
        setLoading(true);

        // Plano premium e ;
        if (me?.currentMainCompany?.planType?.toString() !== '3' || me?.currentMainCompany?.companySituation?.toString() === SYSTEM.COMPANY_SITUATION_PENDING_PAYMENT.toString()) {
            setMessages(prev => [...prev, {
                role: 'bot',
                text: `Oi, ${handleGetFirstName(me?.userName)}!<br/><br/><a href='${ROUTES.EMPRESA_USO_E_PLANO}'>Faça um upgrade no seu plano</a> para o <b>premium</b> para usar o assistente virtual! 😸`
            }]);

            return setLoading(false);
        }

        // Filtro de pertinência;
        if (!handleCheckIfIsAboutThePlataform(userMessage)) {
            setMessages(prev => [...prev, { role: 'bot', text: `Desculpe, eu não te entendi muito bem. Mas ainda estou à disposição para auxiliar você por aqui no ${SYSTEM.NAME}. 🤠` }]);
            return setLoading(false);
        }

        // Tópico mais relevante;
        const bestItem = handleFindMostRelevantItem(userMessage, HELP_TOPICS);
        const finalUserMessage = bestItem ? `${userMessage}. Contexto: ${bestItem.description}` : userMessage;

        // Prompt do sistema como primeira mensagem para contexto;
        const SYSTEM_PROMPT_FOR_GEMINI = `
            Você é o assistente oficial do sistema ${SYSTEM.NAME}.
            Regra ABSOLUTA:
            - Responda sempre em no máximo 2 frases.
            - Nunca explique conceitos.
            - Nunca dê exemplos.
            - Nunca peça mais detalhes.
            `.trim();

        const contents = [
            {
                role: 'user',
                parts: [{ text: SYSTEM_PROMPT_FOR_GEMINI }]
            },
            ...conversationRef.current,
            {
                role: 'user',
                parts: [{ text: finalUserMessage }]
            }
        ];

        try {
            const res = await fetch('https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent',
                {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'x-goog-api-key': API_KEY,
                    },
                    body: JSON.stringify({ contents }),
                }
            );

            const data = await res.json();

            const reply = data?.candidates?.[0]?.content?.parts?.[0]?.text ?? 'Erro interno 💀';
            const replyNormalized = reply.replace(/\*\*(.*?)\*\*/g, '<b>$1</b>');

            // Atualiza histórico SEM system prompt;
            conversationRef.current.push(
                { role: 'user', parts: [{ text: userMessage }] },
                { role: 'model', parts: [{ text: reply }] }
            );

            setMessages((p) => [...p, { role: 'bot', text: replyNormalized }]);
        } catch (err) {
            console.error(err);
            setMessages((p) => [...p, { role: 'bot', text: 'Erro ao conectar 😞' }]);
        } finally {
            setLoading(false);
        }
    }

    function handleCheckIfIsAboutThePlataform(text: string) {
        const s = text.toLowerCase();

        const keywords = new Set([
            'oi', 'olá', 'ola', 'tudo bem', 'como vai', 'beleza', 'oxe', 'oxi', 'td bem',
            'blz', 'como você vai', 'agend', 'serviço', 'servico', 'horá', 'hora', 'horario',
            'agendamento', 'evento', 'cancelar', 'cancel', 'marc', 'marcar', 'remarcar',
            'remarc', 'agenda', 'notifica', 'notificação', 'notificacao', 'pagamento',
            'fatura', 'plano', 'empresa', 'cadast', 'login', 'senha', 'checkout',
            'se chama', 'seu nome', 'configura', 'cliente', 'follow',
            'ordem', 'estoque', 'nota', 'ajuda', SYSTEM.NAME, 'obrigad', 'whatsapp', 'zap',
            'consult', 'paciente', 'colaborador', 'equipe', 'sistema', 'suporte',
            'crio', 'excluo', 'registro', 'cadastro', 'criar', 'registrar', 'cadastrar', 'excluir',
            'email', 'e-mail', 'senha', 'esqueci', 'custa', 'mensalidade'
        ]);

        for (const kw of keywords) {
            if (s.includes(kw)) return true;
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
                    showButtonAbsolute && (
                        <Tippy content={TIPPY_CHATBOT} placement='left'>
                            <button className={styles.bubbleButton} onClick={() => setIsOpenChatbot(true)}>
                                <Image src={ImgMaestro} width={30} height={30} alt='' priority={true} />
                            </button>
                        </Tippy>
                    )
                )
            }
        </div>
    )
}

interface iHelpItem {
    title: string;
    description: string;
}

interface iHelpTopic {
    topic: string;
    description: string;
    items: iHelpItem[];
}

function handleNormalize(text: string): string {
    return text.toLowerCase().normalize('NFD').replace(/\p{Diacritic}/gu, '');
}

function handleCleanText(text: string) {
    const STOP_WORDS = [
        'como', 'sua', 'seu', 'seus', 'suas',
        'fazer', 'sabe', 'os', 'as', 'o', 'a', 'um', 'uma',
        'de', 'do', 'da', 'e', 'no', 'na', 'não', 'é', 'já',
        'por', 'ou', 'serao', 'para', 'pelo', 'pela',
        'cada', 'posso', 'tanto', 'voce', 'pode', 'caso',
        'ao', 'aos', 'as', 'que', 'quais', 'qual', 'oi',
        'olá', 'ola', 'tudo', 'bem', 'vai'
    ];

    return handleNormalize(text).replace(/[?.!,]/g, '').split(/\s+/).filter(w => w.length > 2 && !STOP_WORDS.includes(w)).join(' ');
}

function handleFindMostRelevantItem(userMessage: string, topics: iHelpTopic[]): iHelpItem | null {
    const userWords = handleCleanText(userMessage).split(' ').filter(Boolean);
    let bestItem: iHelpItem | null = null;
    let maxScore = 0;

    for (const t of topics) {
        for (const item of t.items) {
            const itemWords = handleCleanText(item.title + ' ' + item.description).split(' ');
            const score = userWords.reduce((acc, w) => acc + (itemWords.includes(w) ? 1 : 0), 0);
            // console.log(itemWords, score)

            if (score > maxScore) {
                maxScore = score;
                bestItem = item;
            }
        }
    }

    return bestItem;
}