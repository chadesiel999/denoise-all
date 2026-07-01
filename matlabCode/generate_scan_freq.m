clear;
close all;
%% 本振系数/MHz
LO1 = 11500;
LO2 = 10500;
LO3 = 14500;
%% 起始频率/MHz
f1 = 6000;
%% 打开保存文件
 fid = fopen('扫频50M-2G.txt','w');
fixedPart_1234 = {'Sub1|Sub2|Sub3|Sub4'};
fixedPart_1 = {'-,-,-,Sub1|-|-|-'};
fixedPart_12 = {'-,-,Sub1|Sub2|-|-'};
fixedPart_2 = {'-,-,-|Sub2|-|-'};
fixedPart_23 = {'-,-|Sub2|Sub3|-'};
fixedPart_3 = {'-,-|-|Sub3|-'};
fixedPart_34 = {'-|-|Sub3|Sub4'};
fixedPart_4 = {'-|-|-|Sub4'};

 %% 全通带
for i = 50:20:2000
    if(i<=2000)
        fstr = sprintf('%dMHz', i);
        row = [{fstr, fstr}, fixedPart_1];
        fprintf(fid, '%s,%s,%s\n', row{:});
%     elseif i==6000
%         fstr = sprintf('%dMHz', i);
%         fstr_lo1=sprintf('%dMHz', LO1-i);
%         row = [{fstr, fstr,fstr_lo1}, fixedPart_12];
%         fprintf(fid, '%s,%s,%s,%s\n',  row{:});
%     if i<11001
%         fstr = sprintf('%dMHz', i);
%         fstr_lo1=sprintf('%dMHz', LO1-i);
%         row = [{fstr,'-',fstr_lo1}, fixedPart_2];
%         fprintf(fid, '%s,%s,%s,%s\n',  row{:});
    end
%     elseif i==11000
%         fstr = sprintf('%dMHz', i);
%         fstr_lo1=sprintf('%dMHz', LO1-i);
%         fstr_lo2=sprintf('%dMHz', i-LO2);
%         row = [{fstr,'-',fstr_lo1,fstr_lo2}, fixedPart_23];
%         fprintf(fid, '%s,%s,%s,%s,%s\n',  row{:});
%     elseif i<16200
%         fstr = sprintf('%dMHz', i);
%         fstr_lo2=sprintf('%dMHz', i-LO2);
%         row = [{fstr,'-','-',fstr_lo2}, fixedPart_3];
%         fprintf(fid, '%s,%s,%s,%s,%s\n',  row{:});
%     elseif i==16200
%         fstr = sprintf('%dMHz', i);
%         fstr_lo2=sprintf('%dMHz', i-LO2);
%         fstr_lo3=sprintf('%dMHz', i-LO3);
%         row = [{fstr,'-','-',fstr_lo2,fstr_lo3}, fixedPart_34];
%         fprintf(fid, '%s,%s,%s,%s,%s,%s\n',  row{:});
%     else
%         fstr = sprintf('%dMHz', i);
%         fstr_lo3=sprintf('%dMHz', i-LO3);
%         row = [{fstr,'-','-','-',fstr_lo3}, fixedPart_4];
%         fprintf(fid, '%s,%s,%s,%s,%s,%s\n',  row{:});
%     end

end
fclose(fid);
 %% 1GHz
%  fid = fopen('Dbi_TiAdc扫频规划_16G_20GHz_3.txt','w');
% for i = 50:50:20000
%      if(i<6000)
%     %     fstr = sprintf('%dMHz', i);
%     %     row = [{fstr, fstr,fstr,fstr,fstr}, fixedPart_1234];
%     %     fprintf(fid, '%s,%s,%s,%s,%s,%s\n', row{:});
% %     elseif i==6000
% %         fstr = sprintf('%dMHz', i);
% %         fstr_lo1=sprintf('%dMHz', LO1-i);
% %         row = [{fstr, fstr,fstr_lo1}, fixedPart_12];
% %         fprintf(fid, '%s,%s,%s,%s\n',  row{:});
%     elseif i<12500
%         % fstr = sprintf('%dMHz', i);
%         % fstr_lo1=sprintf('%dMHz', LO1-i);
%         % row = [{fstr,fstr_lo1,fstr_lo1,fstr_lo1,fstr_lo1}, fixedPart_1234];
%         % fprintf(fid, '%s,%s,%s,%s,%s,%s\n',  row{:});
% %     elseif i==11000
% %         fstr = sprintf('%dMHz', i);
% %         fstr_lo1=sprintf('%dMHz', LO1-i);
% %         fstr_lo2=sprintf('%dMHz', i-LO2);
% %         row = [{fstr,'-',fstr_lo1,fstr_lo2}, fixedPart_23];
% %         fprintf(fid, '%s,%s,%s,%s,%s\n',  row{:});
%     elseif i<17200
%         % fstr = sprintf('%dMHz', i);
%         % fstr_lo2=sprintf('%dMHz', i-LO2);
%         % row = [{fstr,fstr_lo2,fstr_lo2,fstr_lo2,fstr_lo2}, fixedPart_1234];
%         % fprintf(fid, '%s,%s,%s,%s,%s,%s\n',  row{:});
% %     elseif i==16000
% %         fstr = sprintf('%dMHz', i);
% %         fstr_lo2=sprintf('%dMHz', i-LO2);
% %         fstr_lo3=sprintf('%dMHz', i-LO3);
% %         row = [{fstr,'-','-',fstr_lo2,fstr_lo3}, fixedPart_34];
% %         fprintf(fid, '%s,%s,%s,%s,%s,%s\n',  row{:});
%      else
%         fstr = sprintf('%dMHz', i);
%         fstr_lo3=sprintf('%dMHz', i-LO3);
%         row = [{fstr,fstr_lo3,fstr_lo3,fstr_lo3,fstr_lo3}, fixedPart_1234];
%         fprintf(fid, '%s,%s,%s,%s,%s,%s\n',  row{:});
%     end
% % 
% end
 %% 1GHz
% fid = fopen('Dbi_TiAdc扫频规划_2GHz_new.txt','w');
%  for i = 100:5:2000
%     % if(i<=6000)
%         fstr = sprintf('%dMHz', i);
%         fstr_1 = sprintf('%04dMHz', i);
%         row = [{fstr, fstr_1}, fixedPart_1];
%         fprintf(fid, '%s,%s,%s\n', row{:});
    % elseif i>=9100 && i< 11000
    %     fstr = sprintf('%dMHz', i);
    %     fstr_lo1=sprintf('%04dMHz', LO1-i);
    %     row = [{fstr,'-',fstr_lo1}, fixedPart_2];
    %     fprintf(fid, '%s,%s,%s,%s\n',  row{:});
    % elseif i==11000
    %     fstr = sprintf('%dMHz', i);
    %     fstr_lo1=sprintf('%04dMHz', LO1-i);
    %     fstr_lo2=sprintf('%04dMHz', i-LO2);
    %     row = [{fstr,'-',fstr_lo1,fstr_lo2}, fixedPart_23];
    %     fprintf(fid, '%s,%s,%s,%s,%s\n',  row{:});
    % elseif i<=12900 && i>11000
    %     fstr = sprintf('%dMHz', i);
    %     fstr_lo2=sprintf('%04dMHz', i-LO2);
    %     row = [{fstr,'-','-',fstr_lo2}, fixedPart_3];
    %     fprintf(fid, '%s,%s,%s,%s,%s\n',  row{:});
    % elseif i>=16000 && i<=17900
    %     fstr = sprintf('%dMHz', i);
    %     fstr_lo3=sprintf('%04dMHz', i-LO3);
    %     row = [{fstr,'-','-','-',fstr_lo3}, fixedPart_4];
    %     fprintf(fid, '%s,%s,%s,%s,%s,%s\n',  row{:});
    % end

 % end
 % fclose(fid);
  %% 500MHz
% fid = fopen('Dbi_TiAdc扫频规划_20GHz.txt','w');
%  for i = 50:50:20000
%     if(i<=6000)
%         fstr = sprintf('%dMHz', i);
%         row = [{fstr, fstr}, fixedPart_1];
%         fprintf(fid, '%s,%s,%s\n', row{:});
%     elseif i>=10500 && i< 11000
%         fstr = sprintf('%dMHz', i);
%         fstr_lo1=sprintf('%dMHz', LO1-i);
%         row = [{fstr,'-',fstr_lo1}, fixedPart_2];
%         fprintf(fid, '%s,%s,%s,%s\n',  row{:});
%     elseif i==11000
%         fstr = sprintf('%dMHz', i);
%         fstr_lo1=sprintf('%dMHz', LO1-i);
%         fstr_lo2=sprintf('%dMHz', i-LO2);
%         row = [{fstr,'-',fstr_lo1,fstr_lo2}, fixedPart_23];
%         fprintf(fid, '%s,%s,%s,%s,%s\n',  row{:});
%     elseif i<=11500 && i>11000
%         fstr = sprintf('%dMHz', i);
%         fstr_lo2=sprintf('%dMHz', i-LO2);
%         row = [{fstr,'-','-',fstr_lo2}, fixedPart_3];
%         fprintf(fid, '%s,%s,%s,%s,%s\n',  row{:});
%     elseif i>=16000 && i<=16500
%         fstr = sprintf('%dMHz', i);
%         fstr_lo3=sprintf('%dMHz', i-LO3);
%         row = [{fstr,'-','-','-',fstr_lo3}, fixedPart_4];
%         fprintf(fid, '%s,%s,%s,%s,%s,%s\n',  row{:});
%     end
% 
%  end
 fclose(fid);