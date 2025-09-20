'use client';
import ImgMeditation from '@/app/assets/webp/meditation.webp';
import SYSTEM from '@/app/consts/system';
import useTitle from '@/app/hooks/useTitle';
import Image from 'next/image';
import AjudaListCards from './components/list-cards';
import AjudaSearchInput from './components/seach-input';
import styles from './page.module.scss';

export interface iAjudaTopico {
    topic: string;
    description: string;
    items: iAjudaTopicoItem[];
}

export interface iAjudaTopicoItem {
    title: string;
    description: string;
}

export const HELP_TOPICS = [
    {
        topic: 'Criar Conta e Acesso',
        description: `Tudo sobre como criar sua conta, login e segurança de acesso ao ${SYSTEM.NAME}.`,
        items: [
            {
                title: 'Como criar uma conta?',
                description: 'Clique em "Cadastrar", preencha seus dados e confirme o e-mail enviado para ativar sua conta.'
            },
            {
                title: 'Esqueci minha senha',
                description: 'Na tela de login, clique em "Esqueci minha senha" e siga o link enviado para redefinir sua senha.'
            },
            {
                title: 'Acessar em mais de um dispositivo',
                description: 'Sim, basta usar seu e-mail e senha no computador ou celular, o acesso é multi-dispositivo.'
            }
        ]
    },
    {
        topic: 'Clientes',
        description: 'Gerencie o cadastro, histórico e relacionamento com seus clientes.',
        items: [
            {
                title: 'Cadastrar clientes manualmente',
                description: 'Na aba "Clientes", clique em "Adicionar" e preencha os dados como nome, telefone e e-mail.'
            },
            {
                title: 'Histórico do cliente',
                description: 'No perfil do cliente, visualize agendamentos, pagamentos e observações registradas.'
            },
            {
                title: 'Importar lista de clientes',
                description: 'Carregue um arquivo CSV para importar múltiplos clientes de uma vez.'
            }
        ]
    },
    {
        topic: 'Agendamentos',
        description: 'Controle seus horários, serviços e confirmações automáticas.',
        items: [
            {
                title: 'Marcar um agendamento',
                description: 'Acesse "Agendamentos", escolha serviço e profissional e confirme o horário disponível.'
            },
            {
                title: 'Cancelar ou reagendar',
                description: 'Abra o agendamento e selecione "Cancelar" ou "Reagendar". O cliente e o profissional são notificados automaticamente.'
            },
            {
                title: 'Lembretes automáticos',
                description: `O ${SYSTEM.NAME} envia notificações por e-mail e WhatsApp para evitar esquecimentos`
            }
        ]
    },
    {
        topic: 'Financeiro',
        description: 'Acompanhe pagamentos, relatórios e fluxo de caixa da sua empresa.',
        items: [
            {
                title: 'Relatórios financeiros',
                description: 'Veja relatórios de entrada e saída de valores por período para manter controle do seu negócio.'
            },
            {
                title: 'Controle de recebimentos',
                description: `Acompanhe todos os pagamentos feitos pelo ${SYSTEM.NAME}, inclusive pendentes e em processamento.`
            },
            {
                title: 'Reembolsos e ajustes',
                description: 'Se necessário, reembolse um pagamento diretamente pelo histórico financeiro.'
            }
        ]
    },
    {
        topic: 'Membros e Equipe',
        description: 'Gerencie os profissionais da sua equipe e seus acessos.',
        items: [
            {
                title: 'Cadastrar novos membros',
                description: 'Na aba "Membros", clique em "Adicionar" e defina permissões e serviços vinculados.'
            },
            {
                title: 'Definir agenda de profissionais',
                description: 'Cada membro pode ter seus horários de trabalho personalizados no calendário.'
            },
            {
                title: 'Gerenciar permissões de acesso',
                description: 'Controle quem pode acessar informações financeiras, agendamentos e configurações.'
            }
        ]
    },
    {
        topic: 'Preços e Pagamentos',
        description: 'Configure formas de pagamento, planos e emissão de recibos.',
        items: [
            {
                title: 'Ativar pagamentos online',
                description: 'Em "Configurações > Pagamentos", conecte sua conta do provedor (ex: Stripe ou Mercado Pago).'
            },
            {
                title: 'Emitir recibo',
                description: 'Após o pagamento, clique em "Gerar recibo" e envie por e-mail ou imprima.'
            },
            {
                title: 'Definir preços de serviços',
                description: 'Nos cadastros de serviços, insira valores que serão exibidos automaticamente no agendamento.'
            }
        ]
    },
    {
        topic: 'LGPD e Privacidade',
        description: `Saiba como o ${SYSTEM.NAME} protege os dados de clientes e profissionais.`,
        items: [
            {
                title: 'Consentimento de clientes',
                description: 'Durante o cadastro, os clientes aceitam os termos de uso e privacidade conforme a LGPD.'
            },
            {
                title: 'Armazenamento de dados',
                description: 'Os dados são armazenados de forma segura, com criptografia e backups automáticos.'
            },
            {
                title: 'Excluir dados pessoais',
                description: 'Se um cliente solicitar, você pode remover todas as informações associadas ao perfil dele.'
            }
        ]
    },
    {
        topic: 'Módulos',
        description: `Entenda os diferentes módulos disponíveis no ${SYSTEM.NAME} e como utilizá-los.`,
        items: [
            {
                title: 'Quais módulos estão disponíveis?',
                description: `O ${SYSTEM.NAME} possui módulos de Agendamentos, Clientes, Financeiro, Membros e Configurações.`
            },
            {
                title: 'Posso ativar ou desativar módulos?',
                description: 'Sim, em "Configurações > Módulos" você pode ativar apenas os módulos que deseja utilizar.'
            },
            {
                title: 'Novos módulos futuros',
                description: 'Periodicamente adicionamos novos módulos para expandir as funcionalidades da plataforma.'
            }
        ]
    },
    {
        topic: 'Segurança',
        description: 'Veja como protegemos seus dados e garantimos acesso seguro à plataforma.',
        items: [
            {
                title: 'Autenticação segura',
                description: 'Todas as senhas são criptografadas e o sistema conta com autenticação multifator (2FA).'
            },
            {
                title: 'Proteção de dados sensíveis',
                description: 'As informações financeiras e pessoais são transmitidas com criptografia SSL/TLS.'
            },
            {
                title: 'Controle de acessos',
                description: 'Você pode limitar permissões de membros da equipe para proteger dados estratégicos.'
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
                    <Image src={ImgMeditation} alt='' priority={true} />
                </div>
            </div>

            <AjudaSearchInput keySearch='' />
            <AjudaListCards topics={[...HELP_TOPICS].sort((a, b) => a.topic.localeCompare(b.topic))} />
        </section>
    )
}