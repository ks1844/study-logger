using Application.Interfaces;

namespace Infrastructure.Services;

public class AiService : IAiService
{
    // TODO: 実際のAI APIの実装(OpenAI, Claude等)
    // 今はダミー実装
    
    public async Task<string> GeneratePrSummaryAsync(string studyData)
    {
        // ダミーのPR文を生成
        await Task.Delay(100); // API呼び出しをシミュレート
        
        return $"私はこれまでの学習を通じて、技術スキルを向上させることができました。{studyData}を通じて実践的な開発経験を積み、問題解決能力を養いました。今後も継続的に学習し、チームに貢献できるエンジニアを目指します。";
    }

    public async Task<string> GenerateDailyReportAsync(string studyData)
    {
        // ダミーの日報を生成
        await Task.Delay(100); // API呼び出しをシミュレート
        
        return $"本日の学習内容: {studyData}";
    }
}
