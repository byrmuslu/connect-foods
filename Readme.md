Merhabalar, benim için keyifli bir çalışma oldu umarım siz de incelerken keyif alırsınız.

Geliştirme süresi 8-10 saat aralığındadır.

Öncelikle veri tabanı tasarım tarafında çok fazla detay verilmediğinden dolayı standart bir düzen kurguladım.
Detaylar için sözlü olarak görüşebilir ve tasarım üzerinde iyileştirme yapabiliriz.

Proje içerisinde FACADE doğrudan kullanmadım. Ancak "Event Driven" bir yapı izlediğimden
ve kaynak yönetimini bütün olarak ele aldığımdan dolayı - "ResourcesManager" - "GameManager"'a bakarsanız
eğer FACADE tasarım kalıbını dolaylı olarak işlediğimi göreceksinizdir.

State tasarım kalıbını ise yine "Event Driven" yapı dolayısı ile çok fazla sisteme dahil etmeden kullandım diyebilirim.
-Bu durumu istediğiniz taktirde yine sözlü olarak görüşebiliriz.

Mekanik kısma gelecek olursak eğer komşuluklar için "Raycast" kullandım ancak kolay bir şekilde implemantasyon değiştirilerek 
dizi üzerinden de komşuluklar hesaplanabilir. - Fizik kullanımının dezavantajları olduğunu düşünmekle beraber level design tarafında
kolaylık sağladığından dolayı bu tarz oyunlarda "Raycast" kullanmayı tercih etmekteyim. 

NOT : DatabaseDesign.png içersinde veri tabanı tasarımı resim olarak yer almaktadır.
NOT : Builds klasörü içerisinde *.apk yer almaktadır.
NOT : Oyun 1080x1920 çözünürlük baz alınarak yapılmıştır. Çözünürlük için bir çalışmaya gidilmemiştir.

NOT : Kişisel işlerim sebebiyle görsel iyileştirmelere çok fazla odaklanamadım.
