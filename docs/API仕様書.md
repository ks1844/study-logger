# 学習記録アプリ API仕様書（MVP版）

## 認証関連
### ユーザー登録
- URL: /api/auth/register
- Method: POST
- Request Body:
{
  "name": "string",
  "email": "string",
  "password": "string",
  "role": "student|manager"
}
- Response: 作成したユーザー情報（id, name, email, role）

### ログイン
- URL: /api/auth/login
- Method: POST
- Request Body:
{
  "email": "string",
  "password": "string"
}
- Response: トークンとユーザーロール（student|manager）

## カテゴリ管理
### カテゴリ作成
- URL: /api/category
- Method: POST
- Request Body:
{
  "name": "string"
}
- Response: 作成されたカテゴリ情報（id, name）

### カテゴリ一覧取得
- URL: /api/categoryList
- Method: GET
- Response: ログイン中の学習者が作成した、is_deletedがfalseのカテゴリ一覧

### カテゴリ更新
- URL: /api/category/{id}
- Method: PUT
- Request Body:
{
  "name": "string"
}
- Response: 更新後のカテゴリ情報

### カテゴリ削除
- URL: /api/category/{id}/delete
- Method: PUT
- Response: 削除完了通知（is_deletedフラグをtrueに更新する論理削除）

## 学習記録
### 学習記録登録
- URL: /api/studyRecord
- Method: POST
- Request Body:
{
  "date": "string(date)",
  "categoryId": "uuid",
  "studyHour": "float",
  "memo": "string"
}
- Response: 登録された学習記録情報（studyHourは2.5hなどの小数点形式）

### 学習記録一覧取得
- URL: /api/studyRecordList
- Method: GET
- Response: 日付、カテゴリ、学習時間、メモを含む一覧（is_deletedがfalseのもの）

### 学習記録更新
- URL: /api/studyRecord/{id}
- Method: PUT
- Request Body:
{
  "categoryId": "uuid",
  "studyHour": "float",
  "memo": "string"
}
- Response: 更新された学習記録情報

### 学習記録削除
- URL: /api/studyRecord/{id}/delete
- Method: PUT
- Response: 削除完了通知（論理削除）

## 日報
### 日報登録
- URL: /api/dailyReport
- Method: POST
- Request Body:
{
  "date": "string(date)",
  "goal": "string",
  "achieved": "string",
  "struggle": "string",
  "overcame": "string"
}
- Response: 登録された日報情報

### 日報一覧取得
- URL: /api/dailyReportList
- Method: GET
- Response: 過去の日報（目標、達成、苦戦、乗り越え方）の一覧

### 日報更新
- URL: /api/dailyReport/{id}
- Method: PUT
- Request Body:
{
  "goal": "string",
  "achieved": "string",
  "struggle": "string",
  "overcame": "string"
}
- Response: 更新された日報情報

### 日報削除
- URL: /api/dailyReport/{id}/delete
- Method: PUT
- Response: 削除完了通知（論理削除）

## ダッシュボード・自己PR
### 学習統計取得
- URL: /api/dashboardSummary
- Method: GET
- Response: 総学習時間、平均学習時間、カテゴリ別集計データ（表示時に自動着色される）

### 自己PR生成
- URL: /api/selfPr/generate
- Method: POST
- Response: 学習記録からAIにより生成された100文字程度の自己PRテキスト

### 自己PR保存・更新
- URL: /api/selfPr/{id}
- Method: PUT
- Request Body:
{
  "content": "string"
}
- Response: 更新された自己PR情報（学習者は自分のPRのみ編集可能）

## 管理者機能
### 学習者一覧取得
- URL: /api/manager/studentList
- Method: GET
- Response: 管理者が所属する会社に紐づく全学習者の学習状況と自己PR