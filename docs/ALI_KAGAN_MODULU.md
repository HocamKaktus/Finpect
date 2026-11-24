# FINPECT – Ali Kağan modülü ve ortak koyu arayüz

## Neler eklendi?

- `Finpect.Core/Models`: `Company`, `CriterionResult`, `ProfileAnalysis` modelleri.
- `Finpect.Core/Profiles`: Buffett, Lynch ve Graham için 6'şar **eğitim amaçlı** kriter; kalıtım ve override örnekleri.
- `Finpect.Core/Services/InvestorAnalysisService.cs`: kriter puanlama, karşılaştırma, overload.
- `Finpect.Core/Data/DemoCompanies.cs`: tamamı **kurgusal** 7 şirket.
- `Finpect.WinForms`: **çerçevesiz koyu arayüz**, yatay üst menü, örnek dashboard, çalışan yatırımcı analizi ve karşılaştırma ekranı.
- `Finpect.Tests/InvestorAnalysisTests.cs`: mevcut xUnit test projesine ilave testler.

## Neden .NET 11?

Ortak GitHub deposunda `Finpect.Core` ve `Finpect.Tests` zaten `net11.0` hedefli. Yeni WinForms projesi bu yüzden `net11.0-windows` hedeflidir. Mevcut iki `.csproj` dosyası ve `Class1.cs` **değiştirilmez**.

## Windows üzerinde açma

1. `.NET 11 SDK` ve bu hedefi destekleyen Visual Studio sürümünü yükleyin; `.NET masaüstü geliştirme` iş yükü etkin olsun.
2. `Finpect.slnx` dosyasını açın.
3. `Finpect.WinForms` projesini **Set as Startup Project** yapın.
4. `F5` ile çalıştırın. Bir hata olursa Visual Studio *Error List* ekranındaki mesajı ekibe gönderin.
5. Test için **Test Explorer / Run All** veya terminalde `dotnet test Finpect.Tests/Finpect.Tests.csproj` kullanın.

**Durum:** Bu ortamda .NET SDK ve Windows çalışma zamanı bulunmadığından canlı derleme/WinForms çalıştırma henüz doğrulanamadı.

## Diğer ekip üyeleri için entegrasyon sözleşmesi

- **Berke:** `UserControl` olan `RiskPage.cs` ve ayrı hesaplama servisi yazsın.
- **Alp:** `UserControl` olan `SimilarityPage.cs` ve CSV / ML.NET modeli yazsın; gerekirse finansal verileri `Finpect.Core.Models.Company` nesnesine dönüştürsün.
- **Ahmet Faik:** `UserControl` olan `ReportsPage.cs`, SQLite servisleri ve raporlama ekranlarını yazsın.
- Her üye WinForms ekranını yalnızca kendi modülünde hazırlasın. Ortak `MainForm`, `Ui`, var olan şirket modelleri üzerinde koordine olmadan değişiklik yapılmasın.
- Modüller `MainForm.RegisterModule("Risk Simülatörü", new RiskPage())` gibi kaydedilebilir; diğer sekmeler için sırasıyla `Şirket Benzerliği` ve `Raporlar` kullanılabilir.
- Finansal veriler gerçek fiyat değildir; hiçbir gösterim yatırım tavsiyesi, garantili getiri ya da ünlü yatırımcıların gerçek kararı olarak sunulmaz.

## Birleştirme notu

Bu paket, arkadaşların oluşturduğu mevcut `Finpect.Core`, `Finpect.Tests`, `Finpect.slnx` dosya yapısı için hazırlanmıştır. Paket `Finpect.Core/Finpect.Core.csproj`, `Finpect.Tests/Finpect.Tests.csproj` veya `.gitignore` dosyalarını içermez, değiştirmez. Sadece `Finpect.slnx` içerisine WinForms projesi eklenmiştir.
