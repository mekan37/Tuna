-- Tuna initial PostgreSQL schema draft.
-- References:
-- 04-hedef-mimari-veri-modeli.md > 9. PostgreSQL Hedef Veri Modeli
-- 04-hedef-mimari-veri-modeli.md > 12. Aktarim Stratejisi

create schema if not exists core;
create schema if not exists catalog;
create schema if not exists inventory;
create schema if not exists sales;
create schema if not exists purchase;
create schema if not exists finance;
create schema if not exists einvoice;
create schema if not exists tracktrace;
create schema if not exists audit;
create schema if not exists reporting;
create schema if not exists staging_foxpro;

create table if not exists core.companies (
    id uuid primary key,
    code text not null unique,
    name text not null,
    tax_number text,
    created_at timestamptz not null default now()
);

create table if not exists core.branches (
    id uuid primary key,
    company_id uuid not null references core.companies(id),
    code text not null,
    name text not null,
    created_at timestamptz not null default now(),
    unique (company_id, code)
);

create table if not exists core.number_sequences (
    id uuid primary key,
    branch_id uuid not null references core.branches(id),
    document_kind text not null,
    prefix text not null,
    next_number bigint not null,
    updated_at timestamptz not null default now(),
    unique (branch_id, document_kind, prefix)
);

create table if not exists catalog.products (
    id uuid primary key,
    legacy_code text,
    name text not null,
    vat_rate numeric(5,2) not null default 0,
    is_active boolean not null default true,
    created_at timestamptz not null default now(),
    unique (legacy_code)
);

create table if not exists catalog.product_barcodes (
    id uuid primary key,
    product_id uuid not null references catalog.products(id),
    barcode text not null unique,
    source_table text not null default 'BARKOD'
);

create table if not exists inventory.stock_movements (
    id uuid not null,
    branch_id uuid not null references core.branches(id),
    product_id uuid not null references catalog.products(id),
    movement_date date not null,
    quantity numeric(18,3) not null,
    source_module text not null,
    source_document_id uuid,
    created_at timestamptz not null default now(),
    primary key (id, movement_date)
) partition by range (movement_date);

create table if not exists sales.invoices (
    id uuid primary key,
    branch_id uuid not null references core.branches(id),
    invoice_number text not null,
    invoice_date date not null,
    account_code text not null,
    net_total numeric(18,2) not null,
    vat_total numeric(18,2) not null,
    gross_total numeric(18,2) not null,
    status text not null,
    created_at timestamptz not null default now(),
    unique (branch_id, invoice_number)
);

create table if not exists sales.invoice_lines (
    id uuid primary key,
    invoice_id uuid not null references sales.invoices(id),
    line_no integer not null,
    product_id uuid references catalog.products(id),
    quantity numeric(18,3) not null,
    bonus_quantity numeric(18,3) not null default 0,
    unit_price numeric(18,6) not null,
    discount_total numeric(18,2) not null default 0,
    vat_rate numeric(5,2) not null,
    unique (invoice_id, line_no)
);

create table if not exists finance.ledger_entries (
    id uuid primary key,
    branch_id uuid not null references core.branches(id),
    account_code text not null,
    entry_date date not null,
    document_kind text not null,
    document_id uuid,
    debit numeric(18,2) not null default 0,
    credit numeric(18,2) not null default 0,
    description text,
    created_at timestamptz not null default now()
);

create table if not exists einvoice.documents (
    id uuid primary key,
    invoice_id uuid references sales.invoices(id),
    document_kind text not null,
    uuid text,
    tax_number text,
    current_status text not null,
    xml_storage_key text,
    pdf_storage_key text,
    created_at timestamptz not null default now()
);

create table if not exists einvoice.document_status_events (
    id uuid primary key,
    document_id uuid not null references einvoice.documents(id),
    status text not null,
    message text,
    occurred_at timestamptz not null,
    raw_payload jsonb
);

create table if not exists tracktrace.pts_numbers (
    id uuid not null,
    branch_id uuid references core.branches(id),
    product_id uuid references catalog.products(id),
    barcode text not null,
    serial_number text not null,
    lot_number text,
    expiry_date date,
    event_date date not null,
    idempotency_key text not null,
    created_at timestamptz not null default now(),
    primary key (id, event_date),
    unique (idempotency_key, event_date)
) partition by range (event_date);

create table if not exists tracktrace.ykks_events (
    id uuid not null,
    event_date date not null,
    branch_id uuid references core.branches(id),
    source_table text not null,
    event_type text not null,
    payload jsonb not null,
    created_at timestamptz not null default now(),
    primary key (id, event_date)
) partition by range (event_date);

create table if not exists audit.outbox_messages (
    id uuid primary key,
    topic text not null,
    idempotency_key text not null unique,
    payload jsonb not null,
    status text not null default 'pending',
    attempt_count integer not null default 0,
    available_at timestamptz not null default now(),
    created_at timestamptz not null default now(),
    processed_at timestamptz
);

create table if not exists audit.audit_events (
    id uuid primary key,
    actor_user_id uuid,
    module text not null,
    action text not null,
    entity_name text not null,
    entity_id text not null,
    occurred_at timestamptz not null default now(),
    before_value jsonb,
    after_value jsonb
);

create table if not exists staging_foxpro.import_batches (
    id uuid primary key,
    source_root text not null,
    started_at timestamptz not null default now(),
    completed_at timestamptz,
    status text not null,
    notes text
);

create table if not exists staging_foxpro.raw_records (
    batch_id uuid not null references staging_foxpro.import_batches(id),
    source_table text not null,
    source_record_no bigint not null,
    payload jsonb not null,
    imported_at timestamptz not null default now(),
    primary key (batch_id, source_table, source_record_no)
);

create table if not exists staging_foxpro.migration_errors (
    id uuid primary key,
    batch_id uuid not null references staging_foxpro.import_batches(id),
    source_table text not null,
    source_record_no bigint,
    field_name text,
    error_code text not null,
    error_message text not null,
    raw_value text,
    created_at timestamptz not null default now()
);

create index if not exists ix_ledger_entries_account_date on finance.ledger_entries(account_code, entry_date);
create index if not exists ix_sales_invoices_date on sales.invoices(invoice_date);
create index if not exists ix_outbox_pending on audit.outbox_messages(status, available_at);
