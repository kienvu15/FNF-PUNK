using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class SongMap
{
    // cấu trúc để chứa thông tin từng note
    public struct NoteInfo
    {
        public double time;   // thời điểm note xuất hiện (tính bằng giây)
        public int lane;      // lane index (0..3)
        public string type;   // normal, hold_start, hold_end...
    }

    // Hàm đọc file CSV (TextAsset) và trả về list NoteInfo
    public static List<NoteInfo> Parse(TextAsset csv)
    {
        var result = new List<NoteInfo>();

        // tách từng dòng
        var lines = csv.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var raw in lines)
        {
            var line = raw.Trim();

            // bỏ qua dòng trống hoặc dòng comment (bắt đầu bằng #)
            if (line.Length == 0 || line.StartsWith("#"))
                continue;

            // tách theo dấu phẩy
            var parts = line.Split(',');

            // ép kiểu sang time, lane, type
            double t = double.Parse(parts[0].Trim(), CultureInfo.InvariantCulture);
            int lane = int.Parse(parts[1].Trim());
            string type = parts.Length > 2 ? parts[2].Trim() : "normal";

            // thêm vào list
            result.Add(new NoteInfo { time = t, lane = lane, type = type });
        }

        return result;
    }
}
