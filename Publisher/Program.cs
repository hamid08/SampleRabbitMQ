using Publisher.Jobs;
using Quartz;
using Quartz.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCap(config =>
{
    //Transport RabbitMQ
    config.UseRabbitMQ(rq =>
    {
        rq.HostName = "localhost";
        rq.ExchangeName = "cap.default.topic";
        rq.UserName = "guest";
        rq.Password = "guest";
        rq.Port = 5672;
        rq.ConnectionFactoryOptions = opt => { };
    });

    //Storage
    config.UseMongoDB(mg =>
    {
        mg.DatabaseName = "CapTest";
        mg.DatabaseConnection = "mongodb://localhost";


    });

    //Default: cap.queue.{assembly name}
    //نام پیش فرض گروه مصرف کننده
    config.DefaultGroupName= "default";

    //Default: Null
    //اضافه کردن پیشوندهای یکپارچه برای گروه مصرف کننده
    config.GroupNamePrefix = null;

    //Default: Null
    //اضافه کردن پیشوندهای یکپارچه برای نام موضوع/صف
    config.TopicNamePrefix = null;


    //Default: v1
    //برای تعیین نسخه ای از یک پیام برای جداسازی پیام های نسخه های مختلف سرویس استفاده می شود. 
    config.Version = "v1";


    //Default: 60 sec
    //در طول فرآیند ارسال پیام، اگر انتقال پیام با مشکل مواجه شود، CAP سعی می کند دوباره پیام را ارسال کند. این گزینه پیکربندی برای پیکربندی فاصله بین هر تلاش مجدد استفاده می شود.
    config.FailedRetryInterval= TimeSpan.FromMinutes(1).Minutes;


    //Default: false
    //اگر روی true تنظیم شود، از یک قفل توزیع شده مبتنی بر پایگاه داده برای حل مشکل واکشی همزمان داده ها توسط فرآیندهای تکراری با چندین نمونه استفاده می کنیم. این جدول cap.lock را در پایگاه داده ایجاد می کند.
    config.UseStorageLock = true;


    //Default: 300 sec
    //فاصله زمانی پردازشگر جمع آوری پیام های منقضی شده را حذف می کند.
    config.CollectorCleaningInterval = TimeSpan.FromSeconds(30).Seconds;


    //Default: 1
    //تعداد نخ های مصرف کننده، زمانی که این مقدار بیشتر از 1 باشد، ترتیب اجرای پیام را نمی توان تضمین کرد.
    config.ConsumerThreadCount = 1;


    //Default: 50
    //حداکثر تعداد تلاش مجدد با رسیدن به این مقدار، تلاش مجدد متوقف خواهد شد و حداکثر تعداد تلاش های مجدد با تنظیم این پارامتر تغییر خواهد کرد.
    config.FailedRetryCount= 50;


    //Default: NULL
    //پاسخ به تماس آستانه شکست. این عمل زمانی فراخوانی می شود که تلاش مجدد به مقدار تعیین شده توسط FailedRetryCount برسد، می توانید با تعیین این پارامتر برای انجام یک مداخله دستی اعلان دریافت کنید. به عنوان مثال، یک ایمیل یا اعلان ارسال کنید.
    config.FailedThresholdCallback = null;


    //Default: 24*3600 sec (1 days)
    //زمان انقضا (بر حسب ثانیه) پیام موفقیت. هنگامی که پیام با موفقیت ارسال یا مصرف شد، زمانی که زمان به SucceedMessageExpiredAfter ثانیه برسد، از فضای ذخیره سازی پایگاه داده حذف می شود. با تعیین این مقدار می توانید زمان انقضا را تعیین کنید.
    config.SucceedMessageExpiredAfter = TimeSpan.FromSeconds(1).Seconds;


    //Default: 15*24*3600 sec(15 days)
    //زمان انقضا (بر حسب ثانیه) پیام ناموفق. هنگامی که پیام ارسال شد یا مصرف نشد، هنگامی که زمان به FailedMessageExpiredAfter رسید، از فضای ذخیره سازی پایگاه داده حذف می شود. با تعیین این مقدار می توانید زمان انقضا را تعیین کنید.
    config.FailedMessageExpiredAfter = TimeSpan.FromSeconds(1).Seconds;


    //Default: false
    //اگر درست باشد، همه مصرف کنندگان در یک گروه پیام های دریافتی را به کانال خط لوله ارسال خود ارسال می کنند. هر کانال تعداد موضوعات را روی مقدار ConsumerThreadCount تنظیم کرده است.
    config.UseDispatchingPerGroup = false;


    //Default: false， Before version 7.0 the default behavior is true
    //به طور پیش فرض، CAP فقط یک پیام را از صف پیام می خواند، سپس روش اشتراک را اجرا می کند. پس از اتمام اجرا، پیام بعدی را برای اجرا می خواند. اگر روی true تنظیم شود، مصرف کننده برخی از پیام ها را از قبل در صف حافظه واکشی می کند و سپس آن\ها را برای اجرا در مخزن رشته NET توزیع می کند.
    config.EnableConsumerPrefetch = true;


});



builder.Services.AddQuartz(q =>
{
    // Just use the name of your job that you created in the Jobs folder.
    var jobKey = new JobKey("SendEmailJob");
    q.AddJob<SendEmailJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("SendEmailJob-trigger")
        .WithSimpleSchedule(x =>
        
            x.WithIntervalInSeconds(10)
            .WithRepeatCount(5))

        
    //This Cron interval can be described as "run every minute" (when second is zero)
    //.WithCronSchedule("0 * * * *")
    );
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
