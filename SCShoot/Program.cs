﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class Program
{
    // user32.dllからのインポート
    [DllImport("user32.dll")]
    static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll")]
    static extern bool GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("dwmapi.dll")]
    static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);

    // ウィンドウのタイトル取得用のデリゲート
    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    // RECT構造体
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    // DwmGetWindowAttribute用の属性定数
    const int DWMWA_EXTENDED_FRAME_BOUNDS = 9;

    static void Main(string[] args)
    {
        // 引数が足りているかを確認
        if (args.Length < 2)
        {
            Console.WriteLine("使用方法: <WindowTitlePattern> <OutputFolder>");
            return;
        }

        // コマンドライン引数から正規表現パターンと出力フォルダを取得
        string titlePattern = args[0];
        string outputFolder = args[1];
        Regex regex = new Regex(titlePattern, RegexOptions.IgnoreCase);

        // 出力フォルダが存在しない場合は作成
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        // ウィンドウを列挙して正規表現に一致するものを探す
        IntPtr targetHWnd = IntPtr.Zero;
        EnumWindows((hWnd, lParam) =>
        {
            StringBuilder windowText = new StringBuilder(256);
            GetWindowText(hWnd, windowText, 256);
            if (regex.IsMatch(windowText.ToString()))
            {
                targetHWnd = hWnd;
                return false; // 一致するウィンドウが見つかったら列挙を停止
            }
            return true; // 一致しなければ続行
        }, IntPtr.Zero);

        if (targetHWnd == IntPtr.Zero)
        {
            Console.WriteLine("一致するウィンドウが見つかりませんでした。");
            return;
        }

        // DwmGetWindowAttributeを使ってウィンドウの位置とサイズを取得
        RECT rect;
        int result = DwmGetWindowAttribute(targetHWnd, DWMWA_EXTENDED_FRAME_BOUNDS, out rect, Marshal.SizeOf(typeof(RECT)));

        if (result == 0) // 成功
        {
            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;

            // スクリーンショットを取得
            using (Bitmap bitmap = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(new Point(rect.Left, rect.Top), Point.Empty, new Size(width, height));
                }

                // タイムスタンプを取得してファイル名を生成
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string safeWindowTitle = string.Join("_", titlePattern.Split(Path.GetInvalidFileNameChars())); // ファイル名に使えない文字を置換
                string fileName = $"{safeWindowTitle}_{timestamp}.png";
                string filePath = Path.Combine(outputFolder, fileName);

                // 画像を保存
                bitmap.Save(filePath, ImageFormat.Png);
                Console.WriteLine($"スクリーンショットが保存されました: {filePath}");
            }
        }
        else
        {
            Console.WriteLine("ウィンドウの位置を取得できませんでした。");
        }
    }
}
