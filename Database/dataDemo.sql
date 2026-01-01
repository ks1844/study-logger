-- デモデータ投入用SQL（本番投入前の初期表示・動作確認向け。論理削除データは含まない）
USE study_logger;

-- 学習者デモ
INSERT INTO STUDENT (id, name, email, password, is_deleted) VALUES
('660e8400-e29b-41d4-a716-446655440aaa', '山本太郎', 'demo1@example.com', '$2a$11$N9qo8abc', FALSE),
('660e8400-e29b-41d4-a716-446655440aab', '山田花子', 'demo2@example.com', '$2a$11$N9qo8abc', FALSE);

-- 学習カテゴリ（両方にC#、TypeScript。違いも持たせる）
INSERT INTO CATEGORY (id, student_id, name, is_deleted) VALUES
('660e8400-e29b-41d4-a716-446655440aba', '660e8400-e29b-41d4-a716-446655440aaa', 'C#', FALSE),
('660e8400-e29b-41d4-a716-446655440abb', '660e8400-e29b-41d4-a716-446655440aaa', 'SQL', FALSE),
('660e8400-e29b-41d4-a716-446655440abc', '660e8400-e29b-41d4-a716-446655440aaa', 'TypeScript', FALSE),
('660e8400-e29b-41d4-a716-446655440abd', '660e8400-e29b-41d4-a716-446655440aab', 'C#', FALSE),
('660e8400-e29b-41d4-a716-446655440abe', '660e8400-e29b-41d4-a716-446655440aab', 'JavaScript', FALSE),
('660e8400-e29b-41d4-a716-446655440abf', '660e8400-e29b-41d4-a716-446655440aab', 'TypeScript', FALSE);

-- 学習記録
INSERT INTO STUDY_RECORD (id, student_id, category_id, study_date, study_hour, memo, is_deleted) VALUES
('660e8400-e29b-41d4-a716-446655440aca', '660e8400-e29b-41d4-a716-446655440aaa', '660e8400-e29b-41d4-a716-446655440aba', '2024-02-12', 2.5, 'C#のLINQ練習', FALSE),
('660e8400-e29b-41d4-a716-446655440acb', '660e8400-e29b-41d4-a716-446655440aaa', '660e8400-e29b-41d4-a716-446655440abc', '2024-02-13', 1.0, 'TypeScriptの型勉強', FALSE),
('660e8400-e29b-41d4-a716-446655440acc', '660e8400-e29b-41d4-a716-446655440aaa', '660e8400-e29b-41d4-a716-446655440abb', '2024-02-14', 3.0, 'SQL JOIN演習', FALSE),
('660e8400-e29b-41d4-a716-446655440acd', '660e8400-e29b-41d4-a716-446655440aab', '660e8400-e29b-41d4-a716-446655440abe', '2024-02-12', 1.5, 'JavaScriptのDOM', FALSE),
('660e8400-e29b-41d4-a716-446655440ace', '660e8400-e29b-41d4-a716-446655440aab', '660e8400-e29b-41d4-a716-446655440abf', '2024-02-14', 2.0, 'TypeScriptクラス学習', FALSE),
('660e8400-e29b-41d4-a716-446655440acf', '660e8400-e29b-41d4-a716-446655440aab', '660e8400-e29b-41d4-a716-446655440abd', '2024-02-15', 3.5, 'C#例外処理', FALSE);

-- 日報
INSERT INTO DAILY_REPORT (id, student_id, date, goal, achieved, struggle, overcame, is_deleted) VALUES
('660e8400-e29b-41d4-a716-446655440ada', '660e8400-e29b-41d4-a716-446655440aaa', '2024-02-12', 'SQL学習', 'JOINを習得', 'LEFT JOINで詰まる', 'ネット検索と公式Docで理解', FALSE),
('660e8400-e29b-41d4-a716-446655440adb', '660e8400-e29b-41d4-a716-446655440aab', '2024-02-12', 'C#復習', 'LINQが書けた', 'Where句の書き方', 'サンプル集で確認', FALSE);

-- 自己PR
INSERT INTO PR_SUMMARY (id, student_id, content, is_deleted) VALUES
('660e8400-e29b-41d4-a716-446655440aea', '660e8400-e29b-41d4-a716-446655440aaa', 'C#、SQLの実践力とTypeScriptの保守的コーディングに自信有り', FALSE),
('660e8400-e29b-41d4-a716-446655440aeb', '660e8400-e29b-41d4-a716-446655440aab', 'C#を活かしたWebアプリ開発経験とJS/TSでのフロント構築力', FALSE);

-- 会社デモ
INSERT INTO COMPANY (id, name) VALUES
('660e8400-e29b-41d4-a716-446655440aff', 'デモ株式会社');

-- マネージャデモ
INSERT INTO MANAGER (id, name, email, password, company_id, is_deleted) VALUES
('660e8400-e29b-41d4-a716-446655440b00', '佐々木管理者', 'demomgr@example.com', '$2a$11$N9qo8abc', '660e8400-e29b-41d4-a716-446655440aff', FALSE);

-- 所属関連デモ
INSERT INTO STUDENT_COMPANY (id, student_id, company_id, is_deleted) VALUES
('660e8400-e29b-41d4-a716-446655440b10', '660e8400-e29b-41d4-a716-446655440aaa', '660e8400-e29b-41d4-a716-446655440aff', FALSE),
('660e8400-e29b-41d4-a716-446655440b11', '660e8400-e29b-41d4-a716-446655440aab', '660e8400-e29b-41d4-a716-446655440aff', FALSE);

