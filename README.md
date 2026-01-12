# Study Logger - 学習記録アプリ

IT企業未経験者向けの学習記録管理Webアプリケーション

## 概要

学習者が自身の学習状況を記録・可視化できるアプリケーションです。

### 主な機能

- **ユーザー認証**: メール＋パスワードでログイン（学習者/管理者）
- **学習カテゴリ管理**: 学習分野のカテゴリを作成・編集
- **学習記録**: 日付、カテゴリ、学習時間、メモを記録
- **ダッシュボード**: 今日/今月の学習時間を可視化
- **日報機能**: 目標・達成・苦戦・解決方法を記録

## 技術スタック

### バックエンド
- **フレームワーク**: ASP.NET Core 8.0
- **言語**: C# 12
- **アーキテクチャ**: クリーンアーキテクチャ（4層構造）
  - Domain層: エンティティとビジネスルール
  - Application層: ユースケースとDTO
  - Infrastructure層: DB・外部API
  - Web層: Razor Pages

### フロントエンド
- **テンプレートエンジン**: Razor Pages
- **CSSフレームワーク**: Bootstrap 5
- **JavaScript**: jQuery

### データベース
- **RDBMS**: MySQL 9.0.1
- **ORM**: Entity Framework Core 8.0

### 認証
- **認証方式**: Cookie認証
- **パスワードハッシュ**: BCrypt

## プロジェクト構成

```
study-logger/
├── src/
│   ├── domain/              # ドメイン層
│   │   └── Entities/        # エンティティ
│   ├── application/         # アプリケーション層
│   │   ├── DTOs/            # データ転送オブジェクト
│   │   ├── Interfaces/      # リポジトリインターフェース
│   │   └── Services/        # ユースケース
│   ├── infrastructure/      # インフラ層
│   │   ├── Persistence/     # DB設定
│   │   ├── Repositories/    # リポジトリ実装
│   │   └── Services/        # 外部サービス実装
│   └── web-app/             # Web層
│       ├── Pages/           # Razor Pages
│       └── wwwroot/         # 静的ファイル
├── database/                # データベーススクリプト
├── docs/                    # ドキュメント
└── tests/                   # テスト（今後追加）
```

## セットアップ

### 1. 必要な環境

- .NET 8.0 SDK（8.0.22以降推奨）
- MySQL 9.0.1
- Visual Studio 2022+ / VS Code / Rider

### 2. データベースのセットアップ

```bash
# データベースとテーブルの作成
mysql -u root -p < database/schema.sql

# サンプルデータの投入（オプション）
mysql -u root -p < database/seed_data.sql
```

詳細は [database/README.md](database/README.md) を参照してください。

### 3. 接続文字列の設定

`src/web-app/appsettings.json` を編集:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=study_logger;User=root;Password=your_password;"
  }
}
```

### 4. NuGetパッケージの復元

```bash
cd src/web-app
dotnet restore
```

### 5. アプリケーションの実行

```bash
cd src/web-app
dotnet run
```

ブラウザで `https://localhost:5001` を開く

### 6. 初期ユーザーの作成

アプリケーション起動後、以下のアカウントでログインできます:

**学習者アカウント**
- メール: student@example.com
- パスワード: password

**管理者アカウント**
- メール: admin@example.com
- パスワード: password

## 使い方

1. **ログイン**: メールアドレスとパスワードでログイン
2. **カテゴリ作成**: 学習分野のカテゴリを作成
3. **学習記録**: 日々の学習内容を記録
4. **日報作成**: 目標・達成・苦戦・解決方法を記録
5. **ダッシュボード確認**: 学習時間の統計を確認

詳細は [docs/操作手順書.md](docs/操作手順書.md) を参照してください。

## 開発ガイドライン

### コーディング規約

- C#: Microsoft公式のコーディング規約に準拠
- 命名: PascalCase（クラス/メソッド）、camelCase（変数）
- 1メソッドは50行以内を目安

### ブランチ戦略

- `main`: 本番環境
- `develop`: 開発環境
- `feature/*`: 機能開発ブランチ

### コミットメッセージ

```
feat: 新機能追加
fix: バグ修正
docs: ドキュメント更新
refactor: リファクタリング
test: テスト追加
```

## テスト

```bash
# 単体テスト実行
dotnet test

# カバレッジ確認
dotnet test --collect:"XPlat Code Coverage"
```

## デプロイ

### AWS環境（予定）

- **Web**: AWS Elastic Beanstalk
- **DB**: AWS RDS (MySQL)
- **CI/CD**: GitHub Actions

## ライセンス

このプロジェクトはMITライセンスの下で公開されています。

## 貢献

プルリクエストを歓迎します。大きな変更の場合は、まずissueを開いて変更内容を議論してください。

## 作成者

- プロジェクト作成日: 2026年1月
- 仕様駆動開発によるAI支援開発

## 参考資料

- [要件定義書](docs/要件定義書.md)
- [アーキテクチャ設計書](docs/アーキテクチャ設計書.md)
- [ER図](docs/ER図.md)
- [画面設計書](docs/画面設計書.md)
