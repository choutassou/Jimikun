# 手順実行支援ツール「じみくん」
シンプルで地味な手順実行支援ツールです。
本ツールは手順ファイル（自力作成必要）に基づき、必要な操作を一つずつガイドし、操作エビデンスのキャプチャやログ記録を自動化することで、手順実施の正確性と効率を向上させます。

## 主な機能

- **手順進行ガイド**：事前に定義された手順ファイルに基づき、操作を順番にガイドしながら進行します。
- **自動スクリーンショット取得**：手順ごとに対象ウィンドウのスクリーンショットを自動でキャプチャし、エビデンスを保存します。
- **コマンドの自動クリップボードコピー**：コマンドのあるステップでは、コマンドがクリップボードに自動コピーされ、ユーザーはペーストするだけで簡単に操作を行えます。
- **所要時間の測定とレポート生成**：
  - 各手順（大・小）の所要時間を分単位で計測。
  - 全体の手順実施時間も集計し、詳細なレポートとして出力。
- **エラーハンドリングと進捗状況のログ出力**：進行状況とエラーが`System.Diagnostics.Trace`でログに記録され、エビデンスが確認しやすくなっています。

## 便利性
"手順書があるのに手順ファイルの作成が必要ってこと自体は不便でしょう"の疑問もありますが、正確性が高く要求されてる作業（試験、リリース等）にはかなり有益なので是非ご利用ください。

## 手順ファイルの作成方法
RPAツールの実行はいずれ、あらゆる手順ファイルが必要です。下記は、本ツール専用の手順ファイルを作成する例です。
<div style="display: grid; place-items: center;">
  <img src="https://github.com/choutassou/Jimikun/blob/master/instruction-sample.jpg" alt="manual"/>
</div>


## 使用方法

1. **手順ファイルを用意**：Shift_JISエンコーディングで作成したCSVファイルを使用します
 - 手順ファイルの列は、「Step大,Step小,操作,注意事項,操作Window,キャプチャフォルダ,キャプチャー１,キャプチャー２,キャプチャー３,キャプチャー４」と固定されてます。本ツールはヘッダー依存なので、フォーマット変更されたら動けない
2. **手順ファイルを選択**：アプリケーション起動後、手順ファイルのパスを選択するダイアログが表示されるので、実施対象の手順ファイルを選択します。
3. **手順に沿って操作**：各手順を画面に表示し、ユーザーが「次へ」ボタンを押して進行します。各ステップのエビデンスキャプチャやログ記録は自動で行われます。
4. **レポート生成**：手順実施完了後に所要時間をまとめたレポートがテキスト形式で自動出力されます。

## ソース構成

- **Wizard.cs**：手順ファイルの読み込み、ユーザーインターフェースの表示、進行管理。
- **Capture.cs**：対象ウィンドウのスクリーンショットを取得し、ファイルとして保存。
- **Program.cs**：エントリーポイント。ユーザーが手順ファイルを選択できるUIフォームを提供。
- **CsvPathForm.cs**：手順ファイルの選択用ダイアログ。

## 開発環境

- **言語**：C#
- **フレームワーク**：.NET Framework
- **開発環境**：Visual Studio, VSCode

## 依存ライブラリ

- **System.Diagnostics**：進行ログの記録に使用。
- **System.Drawing**：スクリーンショットの取得と画像保存に使用。
- **System.Windows.Forms**：ユーザーインターフェースの実装に使用。

## ライセンス

このプロジェクトはzlibライセンスの下で公開されています。詳細は[LICENSE](./LICENSE)ファイルをご覧ください。
