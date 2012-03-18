![Daruboard](http://cdn-ak.f.st-hatena.com/images/fotolife/d/daruyanagi/20120226/20120226002301.png)

[[Daruboard]] は、ASP.NET MVC 3 製の Wiki っぽいモノです。

## 特徴

*   __[[http://dropbox.com/|Dropbox]] と連携__

    データベースは Dropbox。Dropbox のクライアントソフトを利用すれば、ローカルファイルシステムからコンテンツを更新できます。リビジョン管理も Dropbox 任せ。

*   __Markdown 記法__

    コンテンツを Markdown でサクサク書けます。HTML（断片）を扱うことも可能。

*   __独自拡張書式__

    二重の[]で囲めば、Twitterの引用、Amazonアフィリエイトタグの挿入、はてなフォトライフ画像の挿入などが可能。これからも便利なものは追加していくつもりです。

*   __Twitter Bootstrap__

    CSS フレームワークに[[http://twitter.github.com/bootstrap/|Bootstrap, from Twitter]] を採用。ちょっと見た目がお洒落で、カラムレイアウトにも対応します。

## ToDoと今後の実装予定

1.  <del>下書き機能</del>
2.  <del>検索機能</del>
3.  <del>FlickrHelperの実装</del>
4.  <del>Dropbox の認証機能</del>
5.  ページの編集機能
6.  ファイルアップロード機能
7.  Diff機能
8.  ロールバック機能

## 更新履歴

*   0.1.9（2012/03/18）
    *   ［改善］キャッシュ改善（ファイルリストの取得トリガーをリクエストではなくタイマーに）
    *   ［改善］FeedHelperのキャッシュ改善
*   0.1.8（2012/03/13）
    *   ［改善］Contentのキャッシュ改善
    *   ［改善］RSS出力の不具合修正
*   0.1.7（2012/03/11）
    *   ［追加］サイドバー記法（「行頭---改行」以下がサイドバーとして解釈される）
*   0.1.6（2012/03/11）
    *   ［修正］脚注記法の処理における不具合
*   0.1.5（2012/03/04）
    *   YouTubeHelperの実装 - [YouTube の URL を動画タグへ変換する（oEmbed） - だるろぐ](http://daruyanagi.hatenablog.com/entry/2012/03/06/011745)
    *   FlickrHelperのビデオ対応
    *   FeedHelperの改良
*   0.1.4（2012/03/03）
    *   新規作成機能の予備的実装
    *   FlickrHelperの実装 - [Flickr の URL を画像タグへ変換する（oEmbed） - だるろぐ](http://daruyanagi.hatenablog.com/entry/2012/03/03/225037)
*   0.1.3（2012/02/28）
    *   ログイン機能の追加 - [ASP.NET MVC 3 で Dropbox の OAuth 認証を使う - だるろぐ](http://daruyanagi.hatenablog.com/entry/2012/02/28/012843)
    *   キャッシュ機能の改善（ビュー → モデル）
    *   下書き機能（Page モデルに IsDraft プロパティを追加）
    *   検索機能の追加（Googleカスタムサーチ）
*   0.1.2（2012/02/27）
    *   記事リストを日付順に並び替え
    *   下書き機能の予備的な実装
*   0.1.1（2012/02/26）
    *   Amazon Helper の修正（APIのバージョンアップ）
    *   資格情報をソースコードではなく Web.config に記述するように修正
    *   CSSの微調整
*   0.1（2012/02/25）
    *   Initial Release.

---

### ダウンロードとインストール

v1.0 になれば公開する予定。
