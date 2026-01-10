erDiagram

    USER ||--o{ STUDY_RECORD : "1対多"
    USER ||--o{ DAILY_REPORT : "1対多"
    USER ||--o{ PR_SUMMARY : "1対多"
    USER ||--o{ CATEGORY : "1対多"

    COMPANY ||--o{ USER : "1対多"

    CATEGORY ||--o{ STUDY_RECORD : "1対多"

    USER {
        int id PK "ユーザーID"
        string name "ユーザー名"
        string email "メールアドレス"
        string password "パスワード（ハッシュ）"
        string role "権限（student/admin）"
        int company_id FK "所属会社ID"
        bool is_deleted "削除フラグ"
    }

    COMPANY {
        int id PK "会社ID"
        string name "会社名"
        bool is_deleted "削除フラグ"
    }

    CATEGORY {
        int id PK "カテゴリID"
        int user_id FK "ユーザーID"
        string name "カテゴリ名"
        bool is_deleted "削除フラグ"
    }

    STUDY_RECORD {
        int id PK "学習記録ID"
        int user_id FK "ユーザーID"
        int category_id FK "カテゴリID"
        float study_hour "学習時間（h）"
        string memo "メモ"
        bool is_deleted "削除フラグ"
    }

    DAILY_REPORT {
        int id PK "日報ID"
        int user_id FK "ユーザーID"
        date date "日付"
        string goal "目標"
        string achieved "達成したこと"
        string struggle "苦戦したこと"
        string overcame "どう乗り越えたか"
        bool is_deleted "削除フラグ"
    }

    PR_SUMMARY {
        int id PK "PR ID"
        int user_id FK "ユーザーID"
        string content "PR本文"
        bool is_deleted "削除フラグ"
    }