# 변경점 v4
- 날짜 기반으로 수정
   - 파일 내 1시간 단위로 집계 후 전체 결과를 날짜 기반으로 재 집계
- 결과 파일명에 실행 시간을 추가

# 변경점 v3
- 날짜 기반이 아닌 파일 기반 날짜 집계
   - 파일내 날짜값중 중간값을 기준으로 날짜를 정함

# 변경점 v2
- 런타임 오류에 대한 try 추가
- 스크립트 진행 출력

# 실행환경
- windows x64

# 실행
- `resourcemonitor.zip`압축 해제
- datasets 폴더 내 CSV 복사
- `.\python\python ./multi_csv.py` 실행

# 결과
- `MultiResults.csv` 파일 생성