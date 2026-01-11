# データベースセットアップ手順

## 前提条件
- MySQL 9.0.1がインストールされていること
- MySQLサーバーが起動していること

## セットアップ手順

### 1. データベースとテーブルの作成

```bash
mysql -u root -p < database/schema.sql
```

### 2. サンプルデータの投入（オプション）

```bash
mysql -u root -p < database/seed_data.sql
```

### 3. 接続文字列の設定

`src/web-app/appsettings.json` で接続文字列を設定してください。

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=study_logger;User=root;Password=your_password;SslMode=None;"
  }
}
```

**重要**: 
- MySQL 9.0.1を使用しています
- ポート番号は通常3306です（変更している場合は適宜修正）
- パスワードは実際の環境に合わせて変更してください

### 4. 初期ユーザーの作成

サンプルデータのパスワードハッシュはダミーです。アプリケーションから以下のユーザーを登録してください:

**学習者アカウント:**
- メール: student@example.com
- パスワード: password
- ロール: student
- 会社ID: 1

**管理者アカウント:**
- メール: admin@example.com
- パスワード: password
- ロール: admin
- 会社ID: 1

## EF Core Migrationsを使用する場合（推奨）

### 1. マイグレーションの作成

```bash
cd src/web-app
dotnet ef migrations add InitialCreate --project ../infrastructure --startup-project .
```

### 2. データベースの更新

```bash
dotnet ef database update --project ../infrastructure --startup-project .
```

## トラブルシューティング

### 接続エラーが発生する場合

1. MySQLサーバーが起動しているか確認
```bash
# macOS/Linux
sudo systemctl status mysql
# または
mysql.server status

# MySQLにログインして確認
mysql -u root -p
```

2. 接続文字列のユーザー名・パスワードが正しいか確認

3. データベース`study_logger`が存在するか確認
```bash
mysql -u root -p -e "SHOW DATABASES;"
```

4. ポート番号を確認
```bash
mysql -u root -p -e "SHOW VARIABLES LIKE 'port';"
```

### SSL接続エラーが発生する場合

接続文字列に `SslMode=None;` を追加してください:
```
Server=localhost;Port=3306;Database=study_logger;User=root;Password=your_password;SslMode=None;
```

### 文字化けが発生する場合

データベースとテーブルの文字コードが`utf8mb4`になっているか確認してください。

```sql
SHOW CREATE DATABASE study_logger;
SHOW CREATE TABLE users;
```

### バージョン確認

MySQL 9.0.1が正しくインストールされているか確認:
```bash
mysql --version
```

## MySQL 9.0.1 の新機能について

MySQL 9.0では以下の機能が追加されています:
- パフォーマンスの向上
- セキュリティの強化
- JSON機能の改善

詳細はMySQL公式ドキュメントを参照してください。
