'use client';
import ImgMeditation from '@/app/assets/webp/meditation.webp';
import SYSTEM from '@/app/consts/system';
import useTitle from '@/app/hooks/useTitle';
import Image from 'next/image';
import AjudaSearchInput from './outros/ajuda.searchInput';
import styles from './page.module.scss';
import AjudaTopics from './topico/topics/ajuda.topicos';

export interface iAjudaTopico {
    topic: string;
    description: string;
    items: iAjudaItem[];
}

export interface iAjudaItem {
    title: string;
    description: string;
}

export const HELP_TOPICS = [
    {
        topic: 'Conta e Acesso',
        description: 'Tudo sobre criação de conta, login e gerenciamento do seu acesso ao Orquestra.',
        items: [
            {
                title: 'Como criar uma conta?',
                description: 'Clique em "Cadastrar" na tela inicial, preencha seus dados e confirme o e-mail enviado para ativar sua conta.'
            },
            {
                title: 'Esqueci minha senha',
                description: 'Na tela de login, clique em "Esqueci minha senha". Você receberá um link no seu e-mail para redefinir a senha.'
            },
            {
                title: 'Posso acessar em mais de um dispositivo?',
                description: 'Sim, o Orquestra funciona tanto no computador quanto no celular, basta entrar com seu e-mail e senha.'
            }
        ]
    },
    {
        topic: 'Agendamentos',
        description: 'Gerencie seus horários, marque consultas e receba lembretes automáticos.',
        items: [
            {
                title: 'Como marcar um horário?',
                description: 'Acesse a aba "Agendamentos", escolha o serviço e o profissional desejado e confirme o horário disponível.'
            },
            {
                title: 'Cancelar ou reagendar',
                description: 'Clique sobre o agendamento e selecione "Cancelar" ou "Reagendar". O sistema notificará automaticamente o profissional.'
            },
            {
                title: 'Recebo lembrete do agendamento?',
                description: 'Sim! O Orquestra envia lembretes por e-mail e WhatsApp para evitar esquecimentos.'
            }
        ]
    },
    {
        topic: 'Serviços e Profissionais',
        description: 'Adicione, gerencie e organize seus serviços e a equipe que os atende.',
        items: [
            {
                title: 'Adicionar novos serviços',
                description: 'No painel administrativo, vá até "Serviços" e clique em "Adicionar". Defina nome, duração e preço do serviço.'
            },
            {
                title: 'Gerenciar equipe',
                description: 'Na seção "Profissionais", você pode cadastrar novos membros da equipe, definir horários e vincular serviços.'
            },
            {
                title: 'Bloquear horários de um profissional',
                description: 'Clique no calendário do profissional e marque o período como indisponível para evitar novos agendamentos.'
            }
        ]
    },
    {
        topic: 'Pagamentos',
        description: 'Configure métodos de pagamento, emita recibos e faça reembolsos facilmente.',
        items: [
            {
                title: 'Como ativar pagamentos online?',
                description: 'No painel, acesse "Configurações > Pagamentos" e conecte sua conta do provedor (ex: Stripe ou Mercado Pago).'
            },
            {
                title: 'Emitir recibo para cliente',
                description: 'Após concluir um pagamento, clique em "Gerar recibo" no histórico e envie por e-mail ou imprima.'
            },
            {
                title: 'Reembolsos',
                description: 'Se precisar reembolsar, acesse o histórico do pagamento e clique em "Reembolsar". O valor será devolvido ao cliente.'
            }
        ]
    },
    {
        topic: 'Configurações',
        description: 'Personalize sua agenda, identidade visual e notificações do Orquestra.',
        items: [
            {
                title: 'Personalizar horários de funcionamento',
                description: 'Em "Configurações > Agenda", defina os horários e dias da semana que sua empresa atende.'
            },
            {
                title: 'Logotipo e identidade visual',
                description: 'Você pode enviar seu logotipo e escolher as cores da interface para deixar o Orquestra com a cara da sua marca.'
            },
            {
                title: 'Notificações automáticas',
                description: 'Ative ou desative lembretes por e-mail e WhatsApp em "Configurações > Notificações".'
            }
        ]
    },
    {
        topic: 'Clientes',
        description: 'Gerencie seus clientes, histórico de agendamentos e importação de listas.',
        items: [
            {
                title: 'Cadastrar clientes manualmente',
                description: 'Na aba "Clientes", clique em "Adicionar". Preencha nome, e-mail e telefone para manter tudo organizado.'
            },
            {
                title: 'Histórico de agendamentos do cliente',
                description: 'Clique no perfil de um cliente para ver todos os agendamentos, pagamentos e observações feitas.'
            },
            {
                title: 'Importar lista de clientes',
                description: 'Você pode importar um arquivo CSV com os dados dos seus clientes para agilizar o processo.'
            }
        ]
    }
] as iAjudaTopico[];

export default function Ajuda() {

    useTitle('Central de ajuda');

    return (
        <section className={styles.main}>
            <div className={styles.hero}>
                <span>Central de ajuda</span>

                <div className={SYSTEM.ANIMATE_PULSE_INFINITE}>
                    <Image src={ImgMeditation} alt='' width={50} height={63} priority={true} />
                </div>
            </div>

            <AjudaSearchInput key='' />

            <AjudaTopics />
        </section>
    )
}