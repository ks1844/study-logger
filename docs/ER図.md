erDiagram

    %% STUDENT：学習者
    STUDENT {
        UUID id PK "学習者ID"
        STRING name "学習者名"
        STRING email "メールアドレス"
        STRING password "パスワード（ハッシュ）"
        BOOLEAN is_deleted "論理削除フラグ"
    }

    %% ADMIN：管理者
    ADMIN {
        UUID id PK "管理者ID"
        STRING name "管理者名"
        STRING email "メールアドレス"
        STRING password "パスワード（ハッシュ）"
        UUID company_id FK "会社ID"
        BOOLEAN is_deleted "論理削除フラグ"
    }

    %% COMPANY：会社
    COMPANY {
        UUID id PK "会社ID"
        STRING name "会社名"
    }

    %% STUDENT_COMPANY：学習者と会社の中間テーブル
    STUDENT_COMPANY {
        UUID id PK "中間テーブルID"
        UUID student_id FK "学習者ID"
        UUID company_id FK "会社ID"
        BOOLEAN is_deleted "論理削除フラグ"
    }

    %% CATEGORY：カテゴリ
    CATEGORY {
        UUID id PK "カテゴリID"
        UUID student_id FK "学習者ID"
        STRING name "カテゴリ名"
        BOOLEAN is_deleted "論理削除フラグ"
    }

    %% STUDY_RECORD：学習記録
    STUDY_RECORD {
        UUID id PK "学習記録ID"
        UUID student_id FK "学習者ID"
        UUID category_id FK "カテゴリID"
        FLOAT study_hour "学習時間"
        TEXT memo "メモ"
        BOOLEAN is_deleted "論理削除フラグ"
    }

    %% DAILY_REPORT：日報
    DAILY_REPORT {
        UUID id PK "日報ID"
        UUID student_id FK "学習者ID"
        DATE date "日付"
        TEXT goal "目標"
        TEXT achieved "達成したこと"
        TEXT struggle "苦戦したこと"
        TEXT overcame "克服したこと"
        BOOLEAN is_deleted "論理削除フラグ"
    }

    %% PR_SUMMARY：自己PR
    PR_SUMMARY {
        UUID id PK "自己PRID"
        UUID student_id FK "学習者ID"
        TEXT content "自己PR内容"
        BOOLEAN is_deleted "論理削除フラグ"
    }

    STUDENT ||--o{ STUDENT_COMPANY : ""
    COMPANY ||--o{ STUDENT_COMPANY : ""
    ADMIN ||--o{ COMPANY : ""
    STUDENT ||--o{ CATEGORY : ""
    STUDENT ||--o{ STUDY_RECORD : ""
    CATEGORY ||--o{ STUDY_RECORD : ""
    STUDENT ||--o{ DAILY_REPORT : ""
    STUDENT ||--o{ PR_SUMMARY : ""