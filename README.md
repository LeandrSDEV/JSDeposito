# 🏪 JSDeposito — Sistema de Gestão de Depósito / Delivery

> **Projeto fullstack profissional**, desenvolvido com foco em **produção real**, **segurança**, **escala** e **boas práticas de mercado**.
> Este sistema simula (e suporta) o funcionamento completo de um depósito/delivery, com múltiplos perfis de acesso, controle de pedidos, frete inteligente, cupons, promoções e dashboards gerenciais.

---

## 🚀 Visão Geral

O **JSDeposito** é uma aplicação **API-first**, com backend robusto em **C# (.NET)** e frontend moderno em **React + TypeScript**, utilizando **PostgreSQL** como banco de dados.

O projeto foi arquitetado pensando em:

* uso em **produção**
* crescimento de usuários
* múltiplos funcionários
* auditoria e controle
* integração futura com **BI (Power BI)**

---

## 🧱 Stack Tecnológica

### Backend

* **C# / .NET (ASP.NET Core)**
* Arquitetura em camadas (Core / Application / API)
* **JWT + Refresh Token**
* **RBAC (Role-Based Access Control)**
* Entity Framework Core
* PostgreSQL (Npgsql)

### Frontend

* **React + Vite**
* **TypeScript**
* SPA desacoplada
* Consumo de API REST
* Guards de rota por autenticação e role

### Banco de Dados

* **PostgreSQL**
* Foco em integridade, performance e analytics
* Preparado para views e integração com BI

---

## 🏗️ Arquitetura

```
JSDeposito/
├── backend/
│   ├── JSDeposito.Core        # Domínio (Entidades, Regras, Enums)
│   ├── JSDeposito.Application # Services / Casos de Uso
│   └── JSDeposito.Api         # Controllers, Auth, Middlewares
│
└── frontend/
    └── jsdeposito-web         # React + Vite + TypeScript
```

### Princípios adotados

* Separação de responsabilidades
* Domínio rico
* API stateless
* Frontend desacoplado
* Segurança no backend

---

## 🔐 Autenticação & Segurança

### Autenticação

* JWT (Access Token)
* Refresh Token
* Tokens stateless
* Claims customizadas (Role e nível)

### Autorização

* `[AllowAnonymous]` para rotas públicas
* `[Authorize]` para rotas protegidas
* `[Authorize(Roles = "...")]` para controle por perfil
* Policies para controle por nível hierárquico

> ⚠️ O frontend **nunca decide permissões** — toda validação é feita no backend.

---

## 👥 Perfis de Acesso (Roles)

O sistema foi projetado para uso real em empresas, com múltiplos níveis de acesso:

| Role           | Descrição                                       |
| -------------- | ----------------------------------------------- |
| ClienteAnonimo | Navega na vitrine, monta pedido e calcula frete |
| Cliente        | Finaliza pedidos e acompanha histórico          |
| Atendente      | Atendimento e suporte ao cliente                |
| Caixa          | Confirma pagamentos                             |
| Repositor      | Controle de estoque                             |
| Gerente        | Acesso a relatórios e dashboards                |
| Dono           | Acesso total ao sistema                         |

Os roles possuem **hierarquia**, permitindo políticas de acesso avançadas.

---

## 🛒 Fluxo de Pedido

1. Cliente (anônimo) navega na vitrine
2. Adiciona produtos ao carrinho
3. Calcula frete (com base em localização)
4. Login é exigido **apenas no checkout**
5. Pedido é criado
6. Endereço é associado
7. Frete é recalculado automaticamente
8. Promoções de frete são aplicadas se ativas
9. Pagamento é confirmado por **Caixa** ou integração futura

---

## 🚚 Frete Inteligente

* Cálculo baseado em distância (Haversine)
* Origem configurável (depósito)
* Integração com geocoding (OpenStreetMap / Nominatim)
* Promoções de frete grátis por período
* Frete recalculado ao alterar endereço

---

## 🎟️ Cupons & Promoções

* Cupons de desconto por valor ou percentual
* Validação automática
* Controle de uso
* Promoções de frete com data de início e fim
* Validação por data e status

---

## 📊 Logs & Auditoria

Toda ação relevante gera **log automático**, incluindo:

* Usuário
* Role
* Ação executada
* Entidade afetada
* Data e hora

Esses logs permitem:

* Auditoria
* Segurança
* Métricas
* Base para bonificações

---

## 📈 Dashboards & BI

* Endpoints preparados para consumo de dados analíticos
* Estrutura compatível com **Power BI**
* Dashboards gerenciais (Gerente / Dono)
* KPIs como:

  * faturamento
  * pedidos por período
  * ticket médio
  * desempenho por funcionário

---

## 🌐 Frontend (React)

### Características

* SPA moderna
* Vitrine pública
* Checkout protegido
* Dashboards por role
* UX focada em conversão

### Estrutura

```
src/
├── api/
├── auth/
├── pages/
│   ├── Vitrine
│   ├── Checkout
│   └── Dashboard
├── components/
├── layouts/
└── types/
```

---

## 🐳 Deploy & Produção

### Backend

* Docker
* Ambiente stateless
* Pronto para cloud (Render, Railway, AWS)

### Frontend

* Build estático
* Deploy em CDN (Vercel, Netlify, Cloudflare Pages)

### Banco

* PostgreSQL gerenciado

---

## 🎯 Objetivo do Projeto

Este projeto foi desenvolvido para:

* Demonstrar domínio em **backend enterprise**
* Mostrar arquitetura moderna fullstack
* Simular um sistema real de mercado
* Servir como base para produto/SaaS
* Ser um **diferencial forte em processos seletivos**

---

## 👨‍💻 Autor

**Leandro**
Desenvolvedor focado em backend, APIs e sistemas escaláveis.

> Projeto construído com foco em qualidade, segurança e visão de negócio.

**LinkDln:**
- https://www.linkedin.com/in/leandro-de-jesus-santos-128478391/
---

## 📌 Status

🚧 Em desenvolvimento contínuo — arquitetura preparada para evolução constante.
