-- 学習記録アプリ データベーススキーマ
-- MySQL用

-- データベース作成（既に存在する場合はスキップ）
CREATE DATABASE IF NOT EXISTS study_logger CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE study_logger;

-- 学習者テーブル
CREATE TABLE IF NOT EXISTS STUDENT (
    id CHAR(36) PRIMARY KEY COMMENT '学習者ID',
    name VARCHAR(255) NOT NULL COMMENT '学習者名',
    email VARCHAR(255) NOT NULL UNIQUE COMMENT 'メールアドレス',
    password VARCHAR(255) NOT NULL COMMENT 'パスワード（ハッシュ）',
    is_deleted BOOLEAN DEFAULT FALSE COMMENT '論理削除フラグ',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '作成日時',
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新日時',
    INDEX idx_email (email),
    INDEX idx_is_deleted (is_deleted)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='学習者';

-- 学習カテゴリテーブル
CREATE TABLE IF NOT EXISTS CATEGORY (
    id CHAR(36) PRIMARY KEY COMMENT 'カテゴリID',
    student_id CHAR(36) NOT NULL COMMENT '学習者ID',
    name VARCHAR(255) NOT NULL COMMENT 'カテゴリ名',
    is_deleted BOOLEAN DEFAULT FALSE COMMENT '論理削除フラグ',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '作成日時',
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新日時',
    FOREIGN KEY (student_id) REFERENCES STUDENT(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_student_id (student_id),
    INDEX idx_is_deleted (is_deleted)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='学習カテゴリ';

-- 学習記録テーブル
CREATE TABLE IF NOT EXISTS STUDY_RECORD (
    id CHAR(36) PRIMARY KEY COMMENT '学習記録ID',
    student_id CHAR(36) NOT NULL COMMENT '学習者ID',
    category_id CHAR(36) NOT NULL COMMENT 'カテゴリID',
    study_date DATE NOT NULL COMMENT '学習日付',
    study_hour FLOAT NOT NULL COMMENT '学習時間（時間単位）',
    memo VARCHAR(200) COMMENT 'メモ',
    is_deleted BOOLEAN DEFAULT FALSE COMMENT '論理削除フラグ',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '作成日時',
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新日時',
    FOREIGN KEY (student_id) REFERENCES STUDENT(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (category_id) REFERENCES CATEGORY(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_student_id (student_id),
    INDEX idx_category_id (category_id),
    INDEX idx_study_date (study_date),
    INDEX idx_is_deleted (is_deleted)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='学習記録';

-- 日報テーブル
CREATE TABLE IF NOT EXISTS DAILY_REPORT (
    id CHAR(36) PRIMARY KEY COMMENT '日報ID',
    student_id CHAR(36) NOT NULL COMMENT '学習者ID',
    date DATE NOT NULL COMMENT '日付',
    goal VARCHAR(200) COMMENT '今日の目標',
    achieved VARCHAR(200) COMMENT '達成したこと',
    struggle VARCHAR(200) COMMENT '苦戦したこと',
    overcame VARCHAR(200) COMMENT '苦戦をどのように乗り越えたか',
    is_deleted BOOLEAN DEFAULT FALSE COMMENT '論理削除フラグ',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '作成日時',
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新日時',
    FOREIGN KEY (student_id) REFERENCES STUDENT(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_student_id (student_id),
    INDEX idx_date (date),
    INDEX idx_is_deleted (is_deleted),
    UNIQUE KEY uk_student_date (student_id, date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='日報';

-- 自己PRテーブル
CREATE TABLE IF NOT EXISTS PR_SUMMARY (
    id CHAR(36) PRIMARY KEY COMMENT '自己PRID',
    student_id CHAR(36) NOT NULL COMMENT '学習者ID',
    content TEXT NOT NULL COMMENT '自己PR内容',
    is_deleted BOOLEAN DEFAULT FALSE COMMENT '論理削除フラグ',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '作成日時',
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新日時',
    FOREIGN KEY (student_id) REFERENCES STUDENT(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_student_id (student_id),
    INDEX idx_is_deleted (is_deleted)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='自己PR';

-- 会社テーブル
CREATE TABLE IF NOT EXISTS COMPANY (
    id CHAR(36) PRIMARY KEY COMMENT '会社ID',
    name VARCHAR(255) NOT NULL COMMENT '会社名',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '作成日時',
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新日時',
    INDEX idx_name (name)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='会社';

-- 管理者テーブル
CREATE TABLE IF NOT EXISTS MANAGER (
    id CHAR(36) PRIMARY KEY COMMENT '管理者ID',
    name VARCHAR(255) NOT NULL COMMENT '管理者名',
    email VARCHAR(255) NOT NULL UNIQUE COMMENT 'メールアドレス',
    password VARCHAR(255) NOT NULL COMMENT 'パスワード（ハッシュ）',
    company_id CHAR(36) NOT NULL COMMENT '会社ID',
    is_deleted BOOLEAN DEFAULT FALSE COMMENT '論理削除フラグ',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '作成日時',
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新日時',
    FOREIGN KEY (company_id) REFERENCES COMPANY(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_email (email),
    INDEX idx_company_id (company_id),
    INDEX idx_is_deleted (is_deleted)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='管理者';

-- 学習者と会社の中間テーブル
CREATE TABLE IF NOT EXISTS STUDENT_COMPANY (
    id CHAR(36) PRIMARY KEY COMMENT '中間テーブルID',
    student_id CHAR(36) NOT NULL COMMENT '学習者ID',
    company_id CHAR(36) NOT NULL COMMENT '会社ID',
    is_deleted BOOLEAN DEFAULT FALSE COMMENT '論理削除フラグ',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '作成日時',
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新日時',
    FOREIGN KEY (student_id) REFERENCES STUDENT(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (company_id) REFERENCES COMPANY(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_student_id (student_id),
    INDEX idx_company_id (company_id),
    INDEX idx_is_deleted (is_deleted),
    UNIQUE KEY uk_student_company (student_id, company_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='学習者と会社の中間テーブル';
