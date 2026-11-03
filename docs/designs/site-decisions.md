# GitHub Pages and discovery decisions

The sibling-project scan covered every one of the 75 direct project directories
under the user's projects folder; the .claude support directory was also checked.
Source matches appeared in 31 directories. Dependency and build directories were
excluded. The full scan inventory remains outside the public repository.

The closest public reference is [MILL5 FastSearch](https://mill5.github.io/FastSearch/),
a static library site with MILL5 and Richard Crane MVP branding, installation,
usage, FAQ, structured data, sitemap and llms.txt. Other reviewed public references
include MILL5 Pineapple and yocto. Local verified MILL5 logo and Richard Crane
portrait assets were reused; the public MVP profile and richcrane.ai establish
person links. No outdated partner designation or performance number was copied.

This site deploys only docs/site through a pinned GitHub Actions Pages workflow.
A Release test gate precedes deployment. Canonical and social URL:
https://mill5.github.io/M5.BloomFilter/. The site has semantic headings, visible
answers matching FAQ JSON-LD, SoftwareSourceCode/Person/Organization entities,
local branding and social images, sitemap, robots file, and source-linked llms.txt.
These aid machine discovery; no search or generated-answer placement is promised.

Browser acceptance covered desktop1440, mobile390 and mobile320 pixel widths:
no overflow or JavaScript errors; the64-bit illustration's check/add/reset paths
passed. The browser demonstration explicitly uses a teaching hash, not the
library's default C# Murmur3KirschMitzenmacher implementation. Exported bits are
documented as detached copies, and the pending source release is distinguished
from the currently published NuGet package.
